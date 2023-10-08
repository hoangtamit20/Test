using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetShop.Data;
using serverapi.Base;
using serverapi.Configurations;
using serverapi.Constants;
using serverapi.Dtos.Merchants;
using serverapi.Dtos.Payments;
using serverapi.Dtos.Payments.VnPay;
using serverapi.Entity;
using serverapi.Enum;
using serverapi.Helpers;

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


        /// <summary>
        /// Get payment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(PaymentDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null)
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Payment with Id : {id} not found!"}});
            return Ok(payment.Adapt<PaymentDto>());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vnPayResponseDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("vnpay-return")]
        [ProducesResponseType(typeof(RedirectResult), (int)HttpStatusCode.Found)]
        public async Task<IActionResult> VnPayReturn([FromQuery]VnPayResponseDto vnPayResponseDto)
        {
            string returnUrl = string.Empty;
            var returnModel = new PaymentReturnDto();
            try
            {
                var isValidSignature = vnPayResponseDto.IsValidSignature(_vnpayConfig.HashSecret);
                if (isValidSignature)
                {
                    var payment = (await _context.Payments.FindAsync(vnPayResponseDto.vnp_TxnRef)).Adapt<PaymentDto>();
                    if (payment is not null)
                    {
                        var merchant = (await _context.Merchants.FindAsync(payment.MerchantId)).Adapt<MerchantInfoDto>();
                        //TODO: create returnUrl
                        returnUrl = merchant?.MerchantReturnUrl ?? string.Empty;
                    }
                    else
                    {
                        returnModel.PaymentStatus = "11";
                        returnModel.PaymentMessage = "Can't find payment at payment service";
                    }         

                    if (vnPayResponseDto.vnp_ResponseCode == "00")
                    {
                        returnModel.PaymentStatus = "00";
                        returnModel.PaymentId = payment!.Id;
                        //TODO: Make signature
                        returnModel.Signature = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        returnModel.PaymentStatus = "10";
                        returnModel.PaymentMessage = "Payment process failed";
                    }
                }
                else
                {
                    returnModel.PaymentStatus = "99";
                    returnModel.PaymentMessage = $"Invalid signature in response!";
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Failed : {ex.Message} "}});
            }
            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
        }

        private async Task<string> GetPaymentDestinationShortName(int paymentDesId)
            => (await _context.PaymentDestinations.FindAsync(paymentDesId))!.DesShortName!;
    }
}