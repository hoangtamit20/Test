using serverapi.Enum;

namespace serverapi.Dtos.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderHistoryInfomationDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShipName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ShipAddress { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ShipPhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MethodPayment { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}