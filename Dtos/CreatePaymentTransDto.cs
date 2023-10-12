
namespace serverapi.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class CreatePaymentTransDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string? TranMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? TranPayload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? TranStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? TranAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? TranDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PaymentId { get; set; }

    }
}