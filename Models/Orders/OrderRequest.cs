using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Models.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ShippingAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ShippingCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? PaymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<OrderDetailRequest>? OrderDetails { get; set; }
    }
}