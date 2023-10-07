using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetShop.Data;
using serverapi.Base;
using serverapi.Configurations;
using serverapi.Constants;
using serverapi.Dtos.Payments;
using serverapi.Dtos.Payments.VnPay;
using serverapi.Entity;

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

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vnpayConfig"></param>
        /// <param name="httpContextAccessor"></param>
        public PaymentController(
            PetShopDbContext context, 
            IOptions<VnPayConfig> vnpayConfig, 
            IHttpContextAccessor httpContextAccessor)
        => (_context, _httpContextAccessor, _vnpayConfig) 
        = (context, httpContextAccessor, vnpayConfig.Value);

        /// <summary>
        /// Payment order with VnPay, Momo, ZaloPay (Authorize)
        /// </summary>
        /// <param name="paymentInfoDto"></param>
        /// <returns></returns>
        /// /// <remarks>
        ///     POST : PaymentDestinationId {1 - VNPAY; 2 - MOMO; 3 - ZALOPAY}
        /// {
        ///     "paymentContent": "THANH TOAN DON HANG 0001",
        ///     "paymentCurrency": "VND",
        ///     "paymentRefId": "ORD1234",
        ///     "requiredAmount": 10000,
        ///     "paymentLanguage": "vn",
        ///     "merchantId": 1,
        ///     "paymentDestinationId": 1,
        ///     "orderId": 1,
        ///     "signValue": "12345ABCD"
        /// }
        /// </remarks>
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
                        case PaymentMethodConstant.VNPAY :
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