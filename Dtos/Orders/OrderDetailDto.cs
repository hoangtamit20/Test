namespace serverapi.Dtos.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailDto : OrderConfirmedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string MethodPayment { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PaymentTile { get; set; } = string.Empty;
    }
}