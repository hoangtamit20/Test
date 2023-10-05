using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Models.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? PaymentMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }
}