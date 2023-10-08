using serverapi.Enum;

namespace serverapi.Dtos.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentReturnDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// 00: Success
        /// 99: Unknown
        /// 10: Error
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PaymentMessage { get; set; }

        /// <summary>
        /// Format: yyyyMMddHHmmss
        /// </summary>
        public string? PaymentDate { get; set; }

        /// <summary>
        /// Mã để Merchant xử lý kết quả
        /// </summary>
        public string? PaymentRefId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Chữ ký để Merchant xác nhận
        /// </summary>
        public string? Signature { get; set; }
    }
}