
namespace serverapi.Services.PaymentService.Momo.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class MomoConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static string ConfigName => "Momo";
        /// <summary>
        /// 
        /// </summary>
        public string PartnerCode { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string IpnUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string AccessKey { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PaymentUrl { get; set; } = string.Empty;
    }
}