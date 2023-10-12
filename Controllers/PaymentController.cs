using System.Net;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PetShop.Data;
using serverapi.Base;
using serverapi.Configurations;
using serverapi.Constants;
using serverapi.Dtos;
using serverapi.Dtos.Merchants;
using serverapi.Dtos.Payments;
using serverapi.Dtos.Payments.VnPay;
using serverapi.Entity;
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
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="vnpayConfig"></param>
        /// <param name="httpContextAccessor"></param>
        
        public PaymentController(
            PetShopDbContext context, 
            IOptions<VnPayConfig> vnpayConfig, 
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        => (_context, _httpContextAccessor, _vnpayConfig, _userManager) 
        = (context, httpContextAccessor, vnpayConfig.Value, userManager);

        /// <summary>
        /// Payment order with VnPay, Momo, ZaloPay (Authorize)
        /// </summary>
        /// <param name="paymentInfoDto"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST : PaymentDestinationId {1 - VNPAY; 2 - MOMO; 3 - ZALOPAY}
        /// {
        ///     "paymentContent": "THANH TOAN DON HANG 0001",
        ///     "paymentCurrency": "VND",
        ///     "requiredAmount": 200000,
        ///     "paymentDate": "2023-10-08T16:24:27.165Z",
        ///     "expireDate": "2023-10-08T16:24:27.165Z",
        ///     "paymentLanguage": "VN",
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
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    var paymentSignature = new PaymentSignature()
                    {
                        SignValue = paymentInfoDto.SignValue,
                        SignDate = DateTime.UtcNow,
                        SignOwn = paymentInfoDto.MerchantId.ToString(),
                        PaymentId = payment.Id,
                        IsValid = true
                    };
                    _context.PaymentSignatures.Add(paymentSignature);
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
                                paymentInfoDto.PaymentCurrency ?? string.Empty,
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
            var b = returnUrl;
            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            var a = $"{returnUrl}?{returnModel.ToQueryString()}";
            System.Console.WriteLine("daiuhdauihd");
            return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
        }

/// <summary>
/// 
/// </summary>
/// <param name="vnPayIpnResponseDto"></param>
/// <returns></returns>

        [HttpGet]
        [Route("check-payment")]
        public async Task<IActionResult>CheckPayment([FromQuery]VnPayIpnResponseDto vnPayIpnResponseDto)
        {
            var result = (await ProcessVnPayIpn(vnPayIpnResponseDto)).Data;
            return Ok(new {
                RspCode = result!.RspCode,
                Message = result.Message
            });
        }


        
        private async Task<BaseResultWithData<VnPayIpnResponseDto>>ProcessVnPayIpn(VnPayIpnResponseDto vnPayIpnResponseDto)
        {
            var result = new BaseResultWithData<VnPayIpnResponseDto>();
            var resultData = new VnPayIpnResponseDto();
            try
            {
                // check valid signature
                var isValidSignature = vnPayIpnResponseDto.IsValidSignature(_vnpayConfig.HashSecret);
                if (isValidSignature)
                {
                    // get payment required
                    var payment = await _context.Payments.FindAsync(vnPayIpnResponseDto.vnp_TxnRef);
                    if (payment != null)
                    {
                        // check amount valid
                        if (payment.RequiredAmount == (vnPayIpnResponseDto.vnp_Amount / 100))
                        {
                            // check payment status
                            if (payment.PaymentStatus != "0")
                            {
                                string message = string.Empty;
                                string status = string.Empty;
                                if (vnPayIpnResponseDto.vnp_ResponseCode == "00" && 
                                    vnPayIpnResponseDto.vnp_TransactionStatus == "00")
                                {
                                    status = "0";
                                    message = "Tran success";
                                }
                                else
                                {
                                    status = "-1";
                                    message = "Tran error";
                                }

                                // create payment trans
                                using (var _transaction = _context.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var paymentTransDto = new CreatePaymentTransDto()
                                        {
                                            PaymentId = vnPayIpnResponseDto.vnp_TxnRef!.Value,
                                            TranMessage = message,
                                            TranDate = DateTime.Now,
                                            TranPayload = JsonConvert.SerializeObject(vnPayIpnResponseDto),
                                            TranStatus = status,
                                            TranAmount = vnPayIpnResponseDto.vnp_Amount
                                        };

                                        var paymentTrans = paymentTransDto.Adapt<PaymentTransaction>();
                                        _context.PaymentTransactions.Add(paymentTrans);
                                        await _context.SaveChangesAsync();

                                        // update payment
                                        payment.PaymentLastMessage = paymentTrans.TranMessage;
                                        payment.PaidAmount = _context.PaymentTransactions
                                            .Where(pt => pt.PaymentId == payment.Id && pt.TranStatus == "0")
                                            .Sum(pt => pt.TranAmount);
                                        payment.PaymentStatus = paymentTrans.TranStatus;
                                        payment.LastUpdateAt = DateTime.Now;
                                        payment.LastUpdateBy = (await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!))!.Id;

                                        _context.Entry<Payment>(payment).State = EntityState.Modified;

                                    }
                                    catch(Exception ex)
                                    {
                                        resultData.Set("99", $"{ex.Message}");
                                        _transaction.Rollback();
                                    }
                                }
                            }
                            else
                            {
                                resultData.Set("02", "Order already confirmed");
                            }
                        }
                        else
                        {
                            resultData.Set("04", "Invalid amount");
                        }
                    }
                    else
                    {
                        resultData.Set("01", "Order not found");
                    }

                }
                else
                {
                    resultData.Set("97", "Invalid Signature");
                }
            }
            catch(Exception ex)
            {
                // TODO: process when exception
                resultData.Set("99", $"Input required data - {ex.Message}");
            }

            result.Data = vnPayIpnResponseDto;
            result.Result = resultData.RspCode == "00";

            return result;
        }

        private async Task<string> GetPaymentDestinationShortName(int paymentDesId)
            => (await _context.PaymentDestinations.FindAsync(paymentDesId))!.DesShortName!;
    }
}