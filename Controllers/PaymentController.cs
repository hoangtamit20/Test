using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PetShop.Data;
using RestSharp;
using serverapi.Base;
using serverapi.Configurations;
using serverapi.Constants;
using serverapi.Dtos;
using serverapi.Dtos.Merchants;
using serverapi.Dtos.Orders;
using serverapi.Dtos.Payments;
using serverapi.Dtos.Payments.Momo;
using serverapi.Dtos.Payments.VnPay;
using serverapi.Entity;
using serverapi.Enum;
using serverapi.Helpers;
using serverapi.Libraries.SignalRs;

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
        private readonly MomoConfig _momoConfig;
        private readonly IHubContext<NotificationHub> _hubContext;

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="vnpayConfig"></param>
        /// <param name="momoConfig"></param>
        /// <param name="hubContext"></param>
        /// <param name="httpContextAccessor"></param>

        public PaymentController(
            PetShopDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<VnPayConfig> vnpayConfig,
            IOptions<MomoConfig> momoConfig,
            UserManager<AppUser> userManager,
            IHubContext<NotificationHub> hubContext
            )
        => (_context, _httpContextAccessor, _vnpayConfig, _momoConfig, _userManager, _hubContext)
        = (context, httpContextAccessor, vnpayConfig.Value, momoConfig.Value, userManager, hubContext);

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
        ///     "paymentLanguage": "VN",
        ///     "paymentDestinationId": 1,
        ///     "orderId": 1
        /// }
        /// </remarks>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentLinkDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentInfoDto paymentInfoDto)
        {
            if (ModelState.IsValid)
            {
                var order = await _context.Orders.FirstOrDefaultAsync(od => od.Id == paymentInfoDto.OrderId);
                if (order is not null && (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Success))
                {
                    return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"The order {paymentInfoDto.OrderId} was paymented"}});
                }
                var result = new BaseResultWithData<PaymentLinkDto>();
                using (var _transaction = await _context.Database.BeginTransactionAsync())
                {
                    var payment = paymentInfoDto.Adapt<Payment>();
                    try
                    {
                        var paymentDest = await _context.PaymentDestinations.FirstOrDefaultAsync(t => t.Id == paymentInfoDto.PaymentDestinationId);

                        payment.MerchantId = paymentInfoDto.MerchantId ??
                            (paymentDest?.DesShortName?.ToLower() == PaymentMethodConstant.VNPAY.ToLower()
                            ? 1 : paymentDest?.DesShortName?.ToLower() == PaymentMethodConstant.MOMO.ToLower() ? 2 : 3);
                        _context.Payments.Add(payment);
                        await _context.SaveChangesAsync();


                        // Tạo ra một chuỗi nối các thông tin trên theo thứ tự quy định
                        string data = $"{payment.MerchantId}{payment.OrderId}{payment.RequiredAmount}{payment.PaymentCurrency}{_vnpayConfig.RefundUrl}";

                        // Tạo ra một đối tượng HMACSHA256 với khóa bí mật
                        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_vnpayConfig.HashSecret));

                        // Tính toán chuỗi băm của data
                        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

                        // Chuyển đổi chuỗi băm thành chuỗi hexa
                        string signValue = BitConverter.ToString(hash).Replace("-", "").ToLower();


                        var paymentSignature = new PaymentSignature()
                        {
                            SignValue = signValue,
                            SignDate = DateTime.UtcNow,
                            SignOwn = paymentInfoDto.MerchantId.ToString(),
                            PaymentId = payment.Id,
                            IsValid = true
                        };
                        _context.PaymentSignatures.Add(paymentSignature);
                        await _context.SaveChangesAsync();

                        // choice method for payment
                        var paymentUrl = string.Empty;
                        switch ((await GetPaymentDestinationShortName(payment.PaymentDestinationId)).ToUpper())
                        {
                            //process for vnpay
                            case PaymentMethodConstant.VNPAY:
                                {
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
                                }
                            //process for momo
                            case PaymentMethodConstant.MOMO:
                                {
                                    var momoOneTimeRequest = new MomoOneTimeRequestDto(
                                        _momoConfig.PartnerCode,
                                        payment.OrderId.ToString(),
                                        (long)paymentInfoDto.RequiredAmount!,
                                        payment.OrderId.ToString(),
                                        paymentInfoDto.PaymentContent ?? string.Empty,
                                        _momoConfig.ReturnUrl,
                                        _momoConfig.IpnUrl,
                                        "captureWallet",
                                        ""
                                    );
                                    momoOneTimeRequest.MakeSignature(_momoConfig.AccessKey, _momoConfig.SecretKey);

                                    var client = new RestClient(_momoConfig.PaymentUrl);
                                    var request = new RestRequest() { Method = Method.Post };
                                    request.AddHeader("Content-Type", "application/json; charset=UTF-8");

                                    request.AddParameter("application/json", JsonConvert.SerializeObject(momoOneTimeRequest), ParameterType.RequestBody);
                                    var response = await client.ExecuteAsync(request);
                                    paymentUrl = JsonConvert.DeserializeObject<MomoOneTimeCreateLinkResponseDto>(response.Content!)?.PayUrl;
                                    break;
                                }

                            default:
                                await _transaction.RollbackAsync();
                                break;
                        }
                        await _transaction.CommitAsync();
                        return Ok(new BaseResultWithData<PaymentLinkDto>()
                        {
                            Success = true,
                            Message = result.Message,
                            Data = new PaymentLinkDto()
                            {
                                PaymentId = payment.Id,
                                PaymentUrl = paymentUrl
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        await _transaction.RollbackAsync();
                        return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Server error - {ex.Message}" } });
                    }

                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList() });
        }




        /// <summary>
        /// Get payment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null)
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Payment with Id : {id} not found!" } });
            return Ok(new BaseResultWithData<PaymentDto>()
            {
                Success = true,
                Message = $"Get payment by id : {id}",
                Data = payment.Adapt<PaymentDto>()
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vnPayResponseDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("vnpay-return")]
        [ProducesResponseType(typeof(RedirectResult), (int)HttpStatusCode.Found)]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayResponseDto vnPayResponseDto)
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
                        var paymenSign = (await _context.PaymentSignatures.FirstOrDefaultAsync(p => p.PaymentId == vnPayResponseDto.vnp_TxnRef))?.SignValue;
                        returnModel.Signature = paymenSign;
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
            catch (Exception ex)
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Failed : {ex.Message} " } });
            }
            // var b = returnUrl;
            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            // var a = $"{returnUrl}?{returnModel.ToQueryString()}";
            // System.Console.WriteLine("daiuhdauihd");
            var redirectUrl = $"{returnUrl}?{returnModel.ToQueryString()}";
            return Redirect(redirectUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="momoOneTimeResultRequestDto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("momo-return")]
        public async Task<IActionResult> MomoReturn(MomoOneTimeResultRequestDto momoOneTimeResultRequestDto)
        {
            var a = await _context.PaymentNotifications.ToListAsync();
            return Ok();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vnPayIpnResponseDto"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("check-vnpay-payment")]
        public async Task<IActionResult> CheckPayment([FromQuery] VnPayIpnResponseDto vnPayIpnResponseDto)
        {
            try
            {
                // check valid signature
                var isValidSignature = vnPayIpnResponseDto.IsValidSignature(_vnpayConfig.HashSecret);
                if (isValidSignature)
                {
                    // get payment required
                    var payment = _context.Payments.Find(vnPayIpnResponseDto.vnp_TxnRef);
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
                                            TranAmount = vnPayIpnResponseDto.vnp_Amount / 100
                                        };

                                        var paymentTrans = paymentTransDto.Adapt<PaymentTransaction>();
                                        _context.PaymentTransactions.Add(paymentTrans);
                                        await _context.SaveChangesAsync();

                                        // update payment
                                        payment.PaymentLastMessage = paymentTrans.TranMessage;
                                        payment.PaidAmount = (_context.PaymentTransactions
                                            .Where(pt => pt.PaymentId == payment.Id && pt.TranStatus == "0")
                                            .Sum(pt => pt.TranAmount));
                                        payment.PaymentStatus = paymentTrans.TranStatus;
                                        payment.LastUpdateAt = DateTime.Now;

                                        // update status for Payment
                                        _context.Entry<Payment>(payment).State = EntityState.Modified;
                                        await _context.SaveChangesAsync();

                                        // update status for Order
                                        var order = await _context.Orders.FindAsync(payment.OrderId);
                                        order!.Status = OrderStatus.Confirmed;
                                        _context.Entry<Order>(order).State = EntityState.Modified;
                                        await _context.SaveChangesAsync();

                                        //remove all product items were payment in customer's cart
                                        var orderDetailOfCurrentOrder = await _context.OrderDetails.Where(odl => odl.OrderId == order.Id).ToListAsync();
                                        foreach (var odDetail in orderDetailOfCurrentOrder)
                                        {
                                            var cartItem = await _context.CartItems.FirstOrDefaultAsync(cartItem => cartItem.ProductId == odDetail.ProductId);
                                            if (cartItem is not null)
                                            {
                                                // check if quantity of product in cart > quantity of order then subst quantity in cart
                                                if (cartItem.Quantity > odDetail.Quantity)
                                                {
                                                    cartItem.Quantity -= odDetail.Quantity;
                                                    _context.Entry<CartItems>(cartItem).State = EntityState.Modified;
                                                }
                                                // remove product in cart
                                                else
                                                {
                                                    _context.CartItems.Remove(cartItem);
                                                }
                                                try
                                                {
                                                    await _context.SaveChangesAsync();
                                                }
                                                catch (Exception ex)
                                                {
                                                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                                                }
                                            }
                                        }
                                        var productIds = orderDetailOfCurrentOrder.Select(odd => odd.ProductId).ToList();
                                        var listCartItemRemove = await _context.CartItems
                                                                                .Where(cartItem =>(productIds != null && productIds.Contains(cartItem.ProductId)))
                                                                                .ToListAsync();
                                        if (listCartItemRemove is not null)
                                        {
                                            _context.CartItems.RemoveRange(listCartItemRemove);
                                            await _context.SaveChangesAsync();
                                        }

                                        // get order comfirmed info
                                        var orderConfirmed = order.Adapt<OrderConfirmedDto>();
                                        var user = await _context.Users.FirstOrDefaultAsync(us => us.Id == order.UserId);
                                        if (user != null)
                                        {
                                            orderConfirmed.Name = user.Name;
                                            orderConfirmed.Email = user.Email!;
                                        }
                                        _transaction.Commit();

                                        // send message to clien when the order was payment successed;
                                        await _hubContext.Clients.All.SendAsync(SignalRConstant.ReceiveNotification, 
                                            $"Customer {orderConfirmed.Name} was payment order {orderConfirmed.Id} successed!");
                                        await _hubContext.Clients.All.SendAsync(SignalRConstant.ReceiveOrderConfirmed, orderConfirmed);

                                        // return for VnPay
                                        return Ok(new { RspCode = "00", Message = "Confirm Success" });
                                    }
                                    catch (Exception ex)
                                    {
                                        _transaction.Rollback();
                                        return Ok(new { RspCode = "99", Message = $"{ex.Message}" });
                                    }
                                }
                            }
                            else
                            {
                                return Ok(new { RspCode = "02", Message = "Order already confirmed" });
                            }
                        }
                        else
                        {
                            return Ok(new { RspCode = "04", Message = "Invalid amount" });
                        }
                    }
                    else
                    {
                        return Ok(new { RspCode = "01", Message = "Order not found" });
                    }
                }
                else
                {
                    return Ok(new { RspCode = "97", Message = "Invalid Signature" });
                }
            }
            catch (Exception ex)
            {
                // TODO: process when exception
                return Ok(new
                {
                    RspCode = "99",
                    Message = $"Input required data - {ex.Message}"
                });
            }
        }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="vnPayRefundDto"></param>
        // /// <returns></returns>
        // [HttpPost]
        // [Route("vnpay-refund")]
        // [ProducesResponseType(typeof(BaseResultWithData<string>), (int)HttpStatusCode.OK)]
        // public async Task<IActionResult> RefundVnPayTransaction(VnPayRefundDto vnPayRefundDto)
        // {
        //     var parameters = new Dictionary<string, string>
        //     {
        //         {"vnp_Version", _vnpayConfig.Version},
        //         {"vnp_Command", "refund"},
        //         {"vnp_TmnCode", _vnpayConfig.TmnCode},
        //         {"vnp_Amount", (vnPayRefundDto.vnp_Amount * 100).ToString()},
        //         {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
        //         {"vnp_TransactionDate", vnPayRefundDto.vnp_TransactionDate.ToString("yyyyMMddHHmmss")},
        //         {"vnp_IpAddr", HttpContext.Connection.RemoteIpAddress!.ToString()},
        //         {"vnp_TxnRef", vnPayRefundDto.vnp_TxnRef.ToString()},
        //         {"vnp_OrderInfo", vnPayRefundDto.vnp_OrderInfo},
        //         // {"vnp_SecureHash", }
        //     };

        //     var vnp_SecureHash = ComputeVnpSecureHash(parameters, _vnpayConfig.HashSecret);
        //     parameters.Add("vnp_SecureHash", vnp_SecureHash);
        //     parameters.Add("vnp_SecureHashType", "SHA256");

        //     using var client = new HttpClient();
        //     var content = new FormUrlEncodedContent(parameters);
        //     var response = await client.PostAsync(_vnpayConfig.RefundUrl, content);

        //     if (response.IsSuccessStatusCode)
        //     {
        //         var responseString = await response.Content.ReadAsStringAsync();
        //         // Parse and handle the response from VNPAY here
        //         return Ok(new BaseResultWithData<string>()
        //         {
        //             Success = true,
        //             Message = "Refund transaction success!",
        //             Data = responseString
        //         }); // Return the response string or a parsed object
        //     }
        //     return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { "Failed to refund transaction" } });
        // }

        // private string ComputeVnpSecureHash(Dictionary<string, string> parameters, string secretKey)
        // {
        //     var signData = secretKey + string.Join("", parameters.OrderBy(d => d.Key).Select(d => d.Key + "=" + HttpUtility.UrlEncode(d.Value)).ToList());
        //     using var sha256 = SHA256.Create();
        //     var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(signData));
        //     return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        // }

        private async Task<string> GetPaymentDestinationShortName(int paymentDesId)
            => (await _context.PaymentDestinations.FindAsync(paymentDesId))!.DesShortName!;
    }
}