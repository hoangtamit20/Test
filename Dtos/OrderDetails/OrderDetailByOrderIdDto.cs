namespace serverapi.Dtos.OrderDetails
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailByOrderIdDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
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
        public decimal SubTotal { get; set; }
    }
}