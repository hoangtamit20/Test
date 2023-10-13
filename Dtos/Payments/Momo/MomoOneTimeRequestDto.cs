using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using serverapi.Helpers;

namespace serverapi.Dtos.Payments.Momo
{
    /// <summary>
    /// 
    /// </summary>
    public class MomoOneTimeRequestDto
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <param name="requestId"></param>
        /// <param name="amount"></param>
        /// <param name="orderId"></param>
        /// <param name="orderInfo"></param>
        /// <param name="redirectUrl"></param>
        /// <param name="ipnUrl"></param>
        /// <param name="requestType"></param>
        /// <param name="extraData"></param>
        /// <param name="lang"></param>
        public MomoOneTimeRequestDto(string partnerCode, string requestId,
            long amount, string orderId, string orderInfo, string redirectUrl,
            string ipnUrl, string requestType, string extraData, string lang = "vi")
        {
            this.PartnerCode = partnerCode;
            this.RequestId = requestId;
            this.Amount = amount;
            this.OrderId = orderId;
            this.OrderInfo = orderInfo;
            this.RedirectUrl = redirectUrl;
            this.IpnUrl = ipnUrl;
            this.RequestType = requestType;
            this.ExtraData = extraData;
            this.Lang = lang;
        }


        /// <summary>
        /// 
        /// </summary>
        public string PartnerCode { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string RequestId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public long Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string OrderInfo { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string RedirectUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string IpnUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string RequestType { get; set; } = "captureWallet";
        /// <summary>
        /// 
        /// </summary>
        public string ExtraData { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Lang { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Signature { get; set; } = string.Empty;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        public void MakeSignature(string accessKey, string secretKey)
        {

            var rawHash = $"accessKey={accessKey}&amount={Amount}&extraData={ExtraData}&ipnUrl={IpnUrl}&orderId={OrderId}&orderInfo={OrderInfo}&partnerCode={PartnerCode}&redirectUrl={RedirectUrl}&requestId={RequestId}&requestType={RequestType}";
            // var rawHash = "accessKey=" + accessKey +
            //     "&amount=" + this.Amount +
            //     "&extraData=" + this.ExtraData +
            //     "&ipnUrl=" + this.IpnUrl +
            //     "&orderId=" + this.OrderId +
            //     "&orderInfo=" + this.OrderInfo +
            //     "&partnerCode=" + this.PartnerCode +
            //     "&redirectUrl=" + this.RedirectUrl +
            //     "&requestId=" + this.RequestId +
            //     "&requestType=" + this.RequestType;
            this.Signature = HasHelper.HmacSHA256(rawHash, secretKey);
        }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="paymentUrl"></param>
        // /// <returns></returns>
        // public (bool, string?) GetLink(string paymentUrl)
        // {
        //     var client = new RestClient(paymentUrl);
        //     var request = new RestRequest() { Method = Method.Post };
        //     request.AddHeader("Content-Type", "application/json; charset=UTF-8");

        // }
    }
}