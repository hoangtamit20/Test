
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Constants;
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
    public class AdminController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="hubContext"></param>
        public AdminController(
            PetShopDbContext context,
            UserManager<AppUser> userManager,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestSignalR()
        {
            await _hubContext.Clients.All.SendAsync(SignalRConstant.ReceiveNotification, $"Anh gửi đến em bằng SignalR nè");
            return Ok();
        }

        /// <summary>
        /// Accept order (Admin)
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("accept-order/{orderId}")]
        public async Task<IActionResult> AcceptOrderOfCustomer(int orderId)
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Order is null!" } });
            var orderExists = await _context.Orders.FindAsync(orderId);
            if (orderExists is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Cannot found order {orderId}" } });
            if (orderExists.Status != OrderStatus.Confirmed)
                return BadRequest(new BaseBadRequestResult()
                    {Errors = new List<string>(){$"Cannot accept order with id {orderId} because the order maybe was payment or not yet payment"}});
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                // change order status
                orderExists.Status = OrderStatus.Success;
                _context.Entry<Order>(orderExists).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
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
        /// <param name="orderId"></param>
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
                    {Errors = new List<string>(){$"Cannot cancel order with id {orderId} because the order was shipped."}});
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
                    listOrderDetail.ForEach(async odl => {
                        var product = await _context.Products.FindAsync(odl.ProductId);
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
                    await _transaction.CommitAsync();
                    return Ok(new BaseResultSuccess(){Message = $"Cancel order {orderId} success"});
                }
                catch(Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"{ex.Message}"}});
                }
            }
        }
    }
}