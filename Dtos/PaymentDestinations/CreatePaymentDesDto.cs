namespace serverapi.Dtos.PaymentDestinations
{
    /// <summary>
    /// 
    /// </summary>
    public class CreatePaymentDesDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string? DesName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? DesShortName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? DesParentId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? DesLogo { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int SortIndex { get; set; }
    }
}