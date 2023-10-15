namespace serverapi.Dtos.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailHistoryDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Thumbnail { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int ProductQuantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ProductPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Subtotal { get; set; }
    }
}