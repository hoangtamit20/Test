using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.Customers;
using serverapi.Entity;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {

        private readonly PetShopDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="petShopDbContext"></param>
        public CustomerController(PetShopDbContext petShopDbContext, UserManager<AppUser> userManager)
        {
            _dbContext = petShopDbContext;
            _userManager = userManager;
        }


        /// <summary>
        /// Get list order history of customer (Authorize)
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("order-history-of-customer")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<List<OrderHistoryInfomationDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> OrdersHistoryOfCustomer(string language)
        {
            language = language ?? "VN";
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is null)
                return Unauthorized(new BaseBadRequestResult(){Errors = new List<string>(){$"Cannot find any information of user"}});
            var orderHistoryInfo = await _dbContext.Orders
                .Include(od => od.OrderDetails)
                .Include(od => od.Payments)
                .ThenInclude(payment => payment.PaymentDestination)
                .Where(od => od.UserId == currentUser.Id)
                .Select(od => new OrderHistoryInfomationDto
                {
                    OrderId = od.Id,
                    ShipName = od.ShipName,
                    ShipAddress = od.ShipAddress,
                    ShipPhoneNumber = od.ShipPhoneNumber,
                    OrderStatus = od.Status,
                    PaymentDate = od.Payments
                        .FirstOrDefault(payment => 
                            payment.OrderId == od.Id)!.PaymentDate!.Value,
                    MethodPayment = od.Payments
                        .FirstOrDefault(payment => 
                            payment.OrderId == od.Id)!.PaymentDestination.DesShortName!,
                    TotalPrice = od.TotalPrice
                })
                .ToListAsync();
            return Ok(new BaseResultWithData<List<OrderHistoryInfomationDto>>()
            {
                Success = true,
                Message = $"List Order history of {currentUser.Name}",
                Data = orderHistoryInfo
            });
        }


        /// <summary>
        /// To view detail of all product was order (Authorize)
        /// </summary>
        /// <param name="language">choose language</param>
        /// <returns></returns>
        [HttpGet]
        [Route("order-detail-of-user")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<List<OrderDetailHistoryDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SeeOrderDetail(string language)
        {
            language = language ?? "VN";
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is null)
            {
                return Unauthorized(new BaseBadRequestResult(){Errors = new List<string>(){$"Cannot found any information of user"}});
            }

            var orderDetails = await _dbContext.OrderDetails
                .Include(odl => odl.Order)
                .Include(odl => odl.Product)
                .ThenInclude(p => p.ProductTranslations)
                .Where(odt => odt.Order.UserId == currentUser.Id)
                .Select(odl => new OrderDetailHistoryDto{
                    ProductName = odl.Product.ProductTranslations
                        .FirstOrDefault(pt => 
                            pt.ProductId == odl.ProductId 
                            && pt.LanguageId == language)!.Name,
                    Thumbnail = odl.Product.Thumbnail,
                    ProductQuantity = odl.Quantity,
                    ProductPrice = odl.SubTotal / odl.Quantity,
                    Subtotal = odl.SubTotal
                }).ToListAsync();
            return Ok(new BaseResultWithData<List<OrderDetailHistoryDto>>()
            {
                Success = true,
                Message = $"List detail of order history",
                Data = orderDetails
            });
        }
    }
}