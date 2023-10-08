using System.Net;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.Orders;
using serverapi.Entity;
using serverapi.Enum;

namespace PetShop.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        public OrderController(PetShopDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Order
        /// <summary>
        /// Get list order
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrders()
        {
            if (_context.Orders == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Db 'Order' is null!" } });
            }
            return Ok((await _context.Orders.ToListAsync()).Adapt<List<OrderInfoDto>>());
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Order/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderInfoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Db 'Order' is null!" } });
            }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Order with id : {id} not found!" } });
            }
            return Ok(order.Adapt<OrderInfoDto>());
        }


        /// <summary>
        /// update order by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateOrderDto"></param>
        /// <returns></returns>
        // PUT: api/Order/5
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutOrder(int id, UpdateOrderDto updateOrderDto)
        {
            if (id != updateOrderDto.Id)
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Route id : {id} not compare OrderId : {updateOrderDto.Id}" } });
            }

            _context.Entry<Order>(updateOrderDto.Adapt<Order>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Route id : {id} not compare OrderId : {updateOrderDto.Id}" } });
                }
                else
                {
                    return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { "Internal Error Server!" } });
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Create order (Authorize)
        /// </summary>
        /// <param name="createOrderDto">Bao gồm : {Thông tin người order; Danh sách sản phẩm đặt hàng}</param>
        /// <returns></returns>
        // POST: api/Order
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<OrderInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> PostOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (createOrderDto.ListProductOrder.IsNullOrEmpty())
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"There are no products to place an order" } });
            }
            var listError = checkValidQuantityOfProductOrder(createOrderDto.ListProductOrder);
            if (listError.Count > 0)
            {
                return BadRequest(new BaseBadRequestResult() { Errors = listError });
            }
            if (_context.Orders == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Db 'Order' is null!" } });
            }

            using (var _transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // process create order
                    var order = createOrderDto.Adapt<Order>();
                    var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (claimValue == null)
                    {
                        // Handle the case where the claim is not found
                        System.Console.WriteLine("skahdahdau");
                    }
                    else
                    {
                        var user = await _userManager.FindByEmailAsync(claimValue);
                        if (user == null)
                        {
                            // Handle the case where the user is not found
                            System.Console.WriteLine("đạioiahd");
                        }
                        else
                        {
                            order.UserId = user.Id;
                            _context.Orders.Add(order);
                            order.Status = OrderStatus.InProgress;
                            await _context.SaveChangesAsync();
                            // Continue with your logic here
                        }
                    }
                    // add product to order details
                    var listOrderDetail = createOrderDto.ListProductOrder.Adapt<List<OrderDetail>>();
                    decimal totalPrice = 0;
                    foreach (var orderDetail in listOrderDetail)
                    {
                        var product = await _context.Products.FindAsync(orderDetail.ProductId);
                        if (product == null)
                        {
                            // Handle the case where the product does not exist
                            listError.Add($"Product with Id : {orderDetail.ProductId} does not exists");
                            continue;
                        }

                        orderDetail.OrderId = order.Id;
                        totalPrice += product.Price * orderDetail.Quantity;
                        orderDetail.SubTotal = product.Price * orderDetail.Quantity;
                    }
                    order.TotalPrice = totalPrice;
                    _context.Entry<Order>(order).State = EntityState.Modified;
                    await _context.OrderDetails.AddRangeAsync(listOrderDetail);
                    await _context.SaveChangesAsync();
                    _transaction.Commit();

                    return Ok(new BaseResultWithData<OrderInfoDto>
                    {
                        Result = true,
                        Message = "Create Order",
                        Data = order.Adapt<OrderInfoDto>()
                    });
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Internal Server Error - {ex.Message}" } });
                }
            }
        }

        /// <summary>
        /// Set status for order by id (Admin)
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <param name="orderStatus">Bao gồm các value : {InProgress; Confirmed; Shipping; Success; Canceled}</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/set-order-status")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetOrderStatus(int id, string orderStatus)
        {
            var orderExists = await _context.Orders.FindAsync(id);
            if (orderExists is null)
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Order with Id : {id} not found" } });

            if (Enum.TryParse<OrderStatus>(orderStatus, true, out var status))
            {
                // Cập nhật trạng thái
                orderExists.Status = status;

                // Lưu thay đổi
                _context.Entry(orderExists).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Invalid status value" } });
            }
        }

        // // DELETE: api/Order/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteOrder(int id)
        // {
        //     if (_context.Orders == null)
        //     {
        //         return NotFound();
        //     }
        //     var order = await _context.Orders.FindAsync(id);
        //     if (order == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Orders.Remove(order);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private List<string> checkValidQuantityOfProductOrder(List<OrderDetailItemsDto> listOrder)
        {
            List<string> Errors = new List<string>();
            listOrder.ForEach(or =>
            {
                var product = _context.Products.Find(or.ProductId);
                if (product is null)
                {
                    Errors.Add($"Product with Id : {or.ProductId} does not exists");
                }
                else
                {
                    if (or.Quantity > product.Stock)
                    {
                        var productName = (_context.ProductTranslations.FirstOrDefault(pt => pt.ProductId == product.Id && pt.LanguageId == "VN"))!.Name;
                        if (product.Stock < 1)
                        {
                            Errors.Add($"Sorry, the store is currently out of product {productName}");
                        }
                        else
                        {
                            Errors.Add($"The quantity of product {productName} left is only: {product.Stock}");
                        }
                    }
                }
            });
            return Errors;
        }
    }
}