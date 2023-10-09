using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.Carts;
using serverapi.Entity;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        public CartController(PetShopDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Api get List cartitems of User to display in your cart (Authorize)
        /// </summary>
        /// <param name="language">Choosed language visible in website</param>
        /// <returns></returns>
        /// <remarks>
        ///     GET :
        /// {
        ///     "id": CartItemsId,
        ///     "productId" : "ProductId",
        ///     "name": "ProductName",
        ///     "price": ProductPrice,
        ///     "quantity": ProductQuantity
        /// }
        /// </remarks>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<CartInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCartsByUserName(string? language)
        {
            if (User.Identity!.Name is null)
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { "Authorize failed" } });
            var cartItems = await _context.CartItems
                .Include(ct => ct.Cart)
                .ThenInclude(c => c.User)
                .Include(ct => ct.Product)
                .ThenInclude(p => p.ProductTranslations)
                .Where(ct => ct.Cart.User.Email == User.Identity!.Name)
                .Select(ct => new CartInfoDto
                {
                    Id = ct.Id,
                    ProductId = ct.ProductId,
                    Name = ct.Product.ProductTranslations
                        .FirstOrDefault(pt => pt.ProductId == ct.ProductId && pt.LanguageId == (language ?? "VN"))!.Name,
                    Price = ct.Product.Price,
                    Quantity = ct.Quantity
                }).ToListAsync();
            return Ok(cartItems);
        }


        /// <summary>
        /// add product to cart (User)
        /// </summary>
        /// <param name="id">Id Product</param>
        /// <returns></returns>
        /// <remarks>
        ///     POST :
        /// {
        ///     "id" : ProductId
        /// }
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AutoAddCartItems([FromBody] int id)
        {
            if (User.Identity!.Name is null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { "Authorize error" } });
            }
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { "product with id : {id} not exists" } });
            var a = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (cart is null) // if cart is null then create cart and add cartitem to cart.
            {
                using (var _transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // add cart
                        cart = new Cart()
                        {
                            UserId = (await _userManager.FindByNameAsync(User.Identity!.Name))!.Id,
                            DateCreated = DateTime.UtcNow
                        };
                        await _context.Carts.AddAsync(cart);
                        await _context.SaveChangesAsync();
                        // add cartitem
                        var cartItem = new CartItems()
                        {
                            ProductId = id,
                            CartId = cart.Id,
                            Quantity = 1
                        };
                        await _context.CartItems.AddAsync(cartItem);
                        await _context.SaveChangesAsync();
                        await _transaction.CommitAsync();
                        return Ok($"Add Product with ID : {id} to cart success");
                    }
                    catch (Exception ex)
                    {
                        await _transaction.RollbackAsync();
                        return StatusCode(500, new BaseBadRequestResult()
                        {
                            Errors = new List<string>()
                            {
                                $"{ex.Message}",
                                "Rollback transaction"
                            }
                        });
                    }
                }
            }
            else // if cart is not null then add cart item to cart
            {
                var cartItemExists = await _context.CartItems.FirstOrDefaultAsync(ct => ct.ProductId == id);
                await _context.CartItems.AddAsync(new CartItems()
                {
                    ProductId = id,
                    CartId = cart.Id,
                    Quantity = cartItemExists is null ? 1 : (cartItemExists.Quantity + 1)
                });
                return Ok($"Add Product with ID : {id} to cart success");
            }
        }


        /// <summary>
        /// Update quantity for product in cart (User)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "User")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateQuantityCartItems(int id, int quantity)
        {
            // check is authorize
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized(new BaseBadRequestResult() { Errors = new List<string>() { "Unauthorized" } });
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with Id : {id} not found!" } });
            if (quantity < 0)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Value of id : {id} in valid!" } });
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItems = await _context.CartItems.FirstOrDefaultAsync(ct => (ct.ProductId == id) && (ct.CartId == cart!.Id));
            // if quantity = 0 -> remove product from cart
            if (quantity == 0)
            {
                try
                {
                    _context.CartItems.Remove(cartItems!);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Internal Server Error - {ex.Message}" } });
                }
            }
            // update quantity for product of cartitem
            cartItems!.Quantity = quantity;
            _context.Entry<CartItems>(cartItems).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Internal Server Error - {ex.Message}" } });
            }
        }


        /// <summary>
        /// Remove all item in cart (User)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "User")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAllCartItems()
        {
            try
            {
                var entityType = _context.Model.FindEntityType(typeof(CartItems));
                var tableName = entityType!.GetTableName();
                var schemaName = entityType!.GetSchema();
                var rawSqlString = $"TRUNCATE TABLE {schemaName}.{tableName}";
                await _context.Database.ExecuteSqlRawAsync(rawSqlString);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Internal Server Error - {ex.Message}" } });
            }
        }

        /// <summary>
        /// Remove item by id (User)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        ///     DELETE :
        /// {
        ///     "id" : "CartItemsId"
        /// }
        /// </remarks>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteCartItemsById(int id)
        {
            var cartItems = await _context.CartItems.FindAsync(id);
            if (cartItems is null)
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){""}});
            try
            {
                _context.CartItems.Remove(cartItems);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Internal Server Error - {ex.Message}" } }); 
            }
        }
    }
}