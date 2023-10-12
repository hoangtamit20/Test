using serverapi.Enum;

namespace serverapi.Dtos.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentContent { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PaymentCurrency { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string PaymentRefId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal? RequiredAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ExpireDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? PaymentLanguage { get; set; } = string.Empty;
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
        public PaymentStatus PaymentStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? PaidAmount { get; set; }
    }
}