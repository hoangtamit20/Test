namespace serverapi.Dtos.Payments.Momo
{
    /// <summary>
    /// 
    /// </summary>
    public class MomoOneTimeCreateLinkResponseDto
    {
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
        public string OrderId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public long Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ResponseTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ResultCode { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PayUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Deeplink { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string QrCodeUrl { get; set; } = string.Empty;
    }
}