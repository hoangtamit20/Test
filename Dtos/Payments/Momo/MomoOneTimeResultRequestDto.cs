using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using serverapi.Helpers;

namespace serverapi.Dtos.Payments.Momo
{
    /// <summary>
    /// 
    /// </summary>
    [BindProperties]
    public class MomoOneTimeResultRequestDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string? partnerCode { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int orderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int requestId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? orderInfo { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? orderType { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? transId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int resultCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? payType { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public long responseTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? extraData { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? signature { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public bool IsValidSignature(string accessKey, string secretKey)
        {
            var rawHash = "accessKey=" + accessKey +
                   "&amount=" + this.amount +
                   "&extraData=" + this.extraData +
                   "&message=" + this.message +
                   "&orderId=" + this.orderId +
                   "&orderInfo=" + this.orderInfo +
                   "&orderType=" + this.orderType +
                   "&partnerCode=" + this.partnerCode +
                   "&payType=" + this.payType +
                   "&requestId=" + this.requestId +
                   "&responseTime=" + this.responseTime +
                   "&resultCode=" + this.resultCode +
                   "&transId=" + this.transId;
            var checkSignature = HasHelper.HmacSHA256(rawHash, secretKey);
            return this.signature!.Equals(checkSignature);
        }
    }
}