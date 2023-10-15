namespace serverapi.Dtos.Payments.VnPay
{
    /// <summary>
    /// 
    /// </summary>
    public class VnPayRefundDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string vnp_Version { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string vnp_Command { get; set; } = "refund";
        /// <summary>
        /// 
        /// </summary>
        public string vnp_TmnCode { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal vnp_Amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime vnp_CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime vnp_TransactionDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vnp_IpAddr { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int vnp_TxnRef { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vnp_OrderInfo { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string vnp_TransactionNo { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string vnp_SecureHash { get; set; } = string.Empty;
    }
}