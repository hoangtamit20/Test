using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentInfoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string PaymentContent { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string? PaymentCurrency { get; set; }

        // /// <summary>
        // /// 
        // /// </summary>
        // public string? PaymentRefId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? RequiredAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? PaymentDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ExpireDate { get; set; } = DateTime.Now.AddMinutes(15);

        /// <summary>
        /// 
        /// </summary>
        public string? PaymentLanguage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MerchantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PaymentDestinationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SignValue { get; set; } = null!;
    }
}