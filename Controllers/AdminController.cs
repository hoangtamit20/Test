using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Constants;
using serverapi.Dtos.Products;
using serverapi.Entity;
using serverapi.Enum;
using serverapi.Libraries.SignalRs;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<NotificationHub> _hubContext;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="hubContext"></param>
        /// <param name="emailSender"></param>
        public AdminController(
            PetShopDbContext context,
            UserManager<AppUser> userManager,
            IHubContext<NotificationHub> hubContext,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _emailSender = emailSender;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestSignalR()
        {
            await _hubContext.Clients.All.SendAsync(SignalRConstant.ReceiveNotification, $"Message send from SignalR");
            return Ok();
        }

        /// <summary>
        /// Accept order (Admin)
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("accept-order/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptOrderOfCustomer(int orderId)
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Order is null!" } });
            var orderExists = await _context.Orders.FindAsync(orderId);
            if (orderExists is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Cannot found order {orderId}" } });
            if (orderExists.Status != OrderStatus.Confirmed)
                return BadRequest(new BaseBadRequestResult()
                { Errors = new List<string>() { $"Cannot accept order with id {orderId} because the order maybe was payment or not yet payment" } });
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                // change order status
                orderExists.Status = OrderStatus.Success;
                _context.Entry<Order>(orderExists).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderExists.UserId);
                    await _emailSender.SendEmailAsync(user?.Email!, $"Accept Order is Successfull!", $"Congratulations {user?.Name}! Your order has been completed!");
                    await _transaction.CommitAsync();
                    return Ok(new BaseResultSuccess() { Message = $"Accept order {orderId} success!" });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
        }

        /// <summary>
        /// Cancel order (Admin)
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel-order/{orderId}")]
        public async Task<IActionResult> CancelOrderOfCustomer(int orderId)
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Order is null!" } });
            var orderExists = await _context.Orders.FindAsync(orderId);
            if (orderExists is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Cannot found order {orderId}" } });
            if (orderExists.Status == OrderStatus.Success)
                return BadRequest(new BaseBadRequestResult()
                { Errors = new List<string>() { $"Cannot cancel order with id {orderId} because the order was shipped." } });
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                //update order status
                orderExists.Status = OrderStatus.Canceled;
                _context.Entry<Order>(orderExists).State = EntityState.Modified;
                //refund product stock
                var listOrderDetail = await _context.OrderDetails
                    .Where(odl => odl.OrderId == orderId)
                    .ToListAsync();
                if (listOrderDetail is not null)
                {
                    listOrderDetail.ForEach(odl =>
                    {
                        var product = _context.Products.Find(odl.ProductId);
                        if (product is not null)
                        {
                            // plus quantity to product stock
                            product.Stock += odl.Quantity;
                            _context.Entry<Product>(product).State = EntityState.Modified;
                        }
                    });
                }
                //TODO : Refund payment for customer
                //todo here
                //

                try
                {
                    await _context.SaveChangesAsync();
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderExists.UserId);
                    await _emailSender.SendEmailAsync(user?.Email!, $"Cancel Order!", $"Sincerely sorry {user?.Name}! Your order has been cancelled.");
                    await _transaction.CommitAsync();
                    return Ok(new BaseResultSuccess() { Message = $"Cancel order {orderId} success" });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
        }

        /// <summary>
        /// Create product, producttranslation, productimages
        /// </summary>
        /// <param name="createProductDto"> Inlcude information of product, producttranslation, and List string of ProductImages for ImagePath </param>
        /// <returns></returns>
        [HttpPost]
        [Route("create-product-admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResultWithData<CreateProductDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (ModelState.IsValid)
            {
                if (
                    _context.Products is null
                    || _context.ProductImages is null
                    || _context.ProductTranslations is null
                    || _context.Languages is null
                )
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Error! Db is null." } });
                using (var _transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        createProductDto.LanguageId = createProductDto.LanguageId ?? "VN";
                        var product = createProductDto.Adapt<Product>();
                        _context.Products.Add(product);
                        await _context.SaveChangesAsync();
                        var productTranslation = createProductDto.Adapt<ProductTranslation>();
                        productTranslation.ProductId = product.Id;
                        _context.ProductTranslations.Add(productTranslation);
                        var productImages = new List<ProductImage>();
                        if (createProductDto.ImageUrls is not null && createProductDto.ImageUrls.Count > 0)
                            createProductDto.ImageUrls.ForEach(image =>
                            {
                                productImages.Add(new ProductImage() { ProductId = product.Id, ImagePath = image });
                            });
                        _context.ProductImages.AddRange(productImages);
                        await _context.SaveChangesAsync();
                        _transaction.Commit();
                        return Ok(new BaseResultWithData<CreateProductDto>()
                        {
                            Success = true,
                            Message = "Create product is success.",
                            Data = createProductDto
                        });
                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                    }
                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList() });
        }

        /// <summary>
        /// update product, producttranslation
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updateProductDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update-product-admin/{productId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDto updateProductDto)
        {
            if (ModelState.IsValid)
            {
                if (productId != updateProductDto.Id)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Route id : {productId} not equal to ProductId {updateProductDto.Id}" } });
                var productExsits = await _context.Products.FindAsync(updateProductDto.Id);
                if (productExsits is null)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with Id {updateProductDto.Id} not found!" } });
                var productTransExists = await _context.ProductTranslations
                    .FirstOrDefaultAsync(pt =>
                        pt.ProductId == productExsits.Id
                        && pt.LanguageId == updateProductDto.LanguageId);
                if (productTransExists is null)
                {
                    var proTrans = updateProductDto.Adapt<ProductTranslation>();
                    proTrans.ProductId = productExsits.Id;
                    _context.ProductTranslations.Add(proTrans);
                }
                else
                {
                    productTransExists = updateProductDto.Adapt<ProductTranslation>();
                    _context.Entry<ProductTranslation>(productTransExists).State = EntityState.Modified;
                }
                productExsits = updateProductDto.Adapt<Product>();
                _context.Entry<Product>(productExsits).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList() });
        }


        /// <summary>
        /// Delete Product by Id (Admin)
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns></returns>

        [HttpDelete]
        [Route("delete-product-admin{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { "Internal server error" } });
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with id : {id} not found" } });
            }
            try
            {
                _context.Products.Remove(product);
                var productTransRemove = await _context.ProductTranslations.FirstOrDefaultAsync(p => p.ProductId == id);
                if (productTransRemove is not null)
                {
                    _context.ProductTranslations.Remove(productTransRemove);
                    using (var _transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            await _context.SaveChangesAsync();
                            await _transaction.CommitAsync();
                            return NoContent();
                        }
                        catch (Exception ex)
                        {
                            await _transaction.RollbackAsync();
                            return StatusCode(500, new BaseBadRequestResult()
                            {
                                Errors = new List<string>()
                            {
                                "An error occurred while performing the transaction",
                                $"Không nên hiển thị lỗi này cho client - {ex.Message}"
                            }
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult()
                {
                    Errors = new List<string>()
                {
                    "Internal server error",
                    $"Không nên hiển thị lỗi này cho client - {ex.Message}"
                }
                });
            }
        }
    }
}