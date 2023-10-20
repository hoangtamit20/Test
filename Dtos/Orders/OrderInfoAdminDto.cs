using serverapi.Enum;

namespace serverapi.Dtos.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderInfoAdminDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? CustomerEmail { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalPrice { get; set; }
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
        public string? ShipEmail { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ShipPhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            return $"{OrderId}, " +
                   $"{OrderDate}, " +
                   $"{CustomerEmail ?? "N/A"}, " +
                   $"{CustomerName}, " +
                   $"{OrderStatus}, " +
                   $"{TotalPrice}, " +
                   $"{ShipName}, " +
                   $"{ShipAddress}, " +
                   $"{ShipEmail ?? "N/A"}, " +
                   $"{ShipPhoneNumber}";
        }
    }
}