using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentLinkDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PaymentLink { get; set; } = string.Empty;
    }
}