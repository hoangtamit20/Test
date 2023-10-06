using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PetShop.Data;
using serverapi.Base;
using serverapi.Configurations;
using serverapi.Dtos.Payments;
using serverapi.Dtos.Payments.VnPay;
using serverapi.Entity;
using serverapi.Models.Orders;
using serverapi.Models.Payments;
using serverapi.Repository.OrderRepository;
using serverapi.Repository.PaymentRepository;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly VnPayConfig _vnpayConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // private readonly IOrderRepository _orderRepository;
        // private readonly IPaymentRepository _paymentRepository;

        // public PaymentController(IOrderRepository orderRepository, IPaymentRepository paymentRepository)
        // {
        //     _orderRepository = orderRepository;
        //     _paymentRepository = paymentRepository;
        // }

        // [HttpPost]
        // [Route("api/v1/payments")]
        // public async Task<ActionResult<Payment>> CreatePayment(PaymentRequest paymentRequest)
        // {
        //     // Validate the request
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     // Get the order
        //     Order order = await _orderRepository.GetOrderByIdAsync(paymentRequest.OrderId);
        //     if (order == null)
        //     {
        //         return NotFound();
        //     }

        //     // Create a new payment
        //     Payment payment = new Payment
        //     {
        //         OrderId = order.Id,
        //         Amount = order.OrderDetails!.AsEnumerable().Sum(od => od.Quantity * od.UnitPrice),
        //         PaymentMethod = paymentRequest.PaymentMethod,
        //         Status = PaymentStatus.Pending.ToString(),
        //     };

        //     // Save the payment
        //     await _paymentRepository.CreatePaymentAsync(payment);

        //     // Update the order status
        //     order.Status = OrderStatus.Processing.ToString();
        //     await _orderRepository.UpdateOrderAsync(order);

        //     // Return the payment
        //     return Ok(payment);
        // }


        // [HttpGet]
        // [Route("api/v1/payments/{paymentId}/verify")]
        // public async Task<ActionResult<Payment>> VerifyPayment(int paymentId)
        // {
        //     // Get the payment
        //     Payment payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
        //     if (payment is null)
        //     {
        //         return NotFound();
        //     }

        //     // Verify the payment
        //     var paymentStatus = payment.Status;
        //     if (paymentStatus == PaymentStatus.Completed.ToString())
        //     {
        //         // Update the order status
        //         payment.Status = PaymentStatus.Completed.ToString();
        //         await _paymentRepository.UpdatePaymentAsync(payment);

        //         // Update the order status
        //         var order = await _orderRepository.GetOrderByIdAsync(payment.OrderId);
        //         order.Status = OrderStatus.Completed.ToString();
        //         await _orderRepository.UpdateOrderAsync(order);
        //     }

        //     // Return the payment
        //     return Ok(payment);
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vnpayConfig"></param>
        /// <param name="httpContextAccessor"></param>
        public PaymentController(PetShopDbContext context, IOptions<VnPayConfig> vnpayConfig, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _vnpayConfig = vnpayConfig.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentLinkDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PaymentWithVnPay([FromBody] PaymentInfoDto paymentInfoDto)
        {
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                var payment = paymentInfoDto.Adapt<Payment>();
                try
                {
                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();

                    var paymentSignature = new PaymentSignature()
                    {
                        SignValue = paymentInfoDto.SignValue,
                        SignDate = DateTime.UtcNow,
                        SignOwn = paymentInfoDto.MerchantId.ToString(),
                        PaymentId = payment.Id,
                        IsValid = true
                    };
                    await _context.PaymentSignatures.AddAsync(paymentSignature);
                    await _context.SaveChangesAsync();
                    await _transaction.CommitAsync();
                    
                    // choice method for payment
                    var paymentUrl = string.Empty;
                    switch (await GetPaymentDestinationShortName(payment.PaymentDestinationId))
                    {
                        case "VNPAY" :
                            var vnpayRequest = new VnPayRequestDto(
                                _vnpayConfig.Version,
                                _vnpayConfig.TmnCode,
                                DateTime.UtcNow,
                                _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
                                paymentInfoDto.RequiredAmount ?? 0,
                                paymentInfoDto.Currency ?? string.Empty,
                                "other",
                                paymentInfoDto.PaymentContent,
                                _vnpayConfig.ReturnUrl,
                                payment.Id.ToString()
                            );

                            paymentUrl = vnpayRequest.GetLink(_vnpayConfig.PaymentUrl, _vnpayConfig.HashSecret);
                            break;
                        default:
                        break;
                    }
                    return Ok(new BaseResultWithData<PaymentLinkDto>()
                    {
                        Result = true,
                        Message = "",
                        Data = new PaymentLinkDto()
                        {
                            PaymentId = payment.Id,
                            PaymentUrl = paymentUrl
                        }
                    });
                }
                catch(Exception ex)
                {
                    return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){$"Server error - {ex.Message}"}});
                }

            }
        }

        private async Task<string> GetPaymentDestinationShortName(int paymentDesId)
            => (await _context.PaymentDestinations.FindAsync(paymentDesId))!.DesShortName!;
    }
}