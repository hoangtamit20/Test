using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class VnPayConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static string ConfigName => "Vnpay";
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string TmnCode { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string HashSecret { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PaymentUrl { get; set;} = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string RefundUrl { get; set; } = string.Empty;
    }
}