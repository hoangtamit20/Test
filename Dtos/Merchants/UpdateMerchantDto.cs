namespace serverapi.Dtos.Merchants
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateMerchantDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        public string? SecretKey { get; set; } = string.Empty;
    }
}