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
        /// Mã giao dịch yêu cầu thanh toán
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PaymentUrl { get; set; } = string.Empty;
    }
}