namespace serverapi.Dtos.Merchants
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateMerchantDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantWebLink { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantReturnUrl { get; set; } = string.Empty;
    }
}