
namespace serverapi.Models.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}