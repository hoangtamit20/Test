using serverapi.Enum;

namespace serverapi.Dtos.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderInfoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShipName { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string ShipAddress { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string? ShipEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShipPhoneNumber { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}