namespace serverapi.Dtos.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderConfirmedDto : OrderInfoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}