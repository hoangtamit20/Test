namespace serverapi.Dtos.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentInfoByOrderIdDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int PaymentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? PaymentContent { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentMethod { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}