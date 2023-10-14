namespace serverapi.Dtos.Payments.VnPay
{
    /// <summary>
    /// 
    /// </summary>
    public class VnPayRefundDto
    {
        public string vnp_Version { get; set; } = string.Empty;

        public string vnp_Command { get; set; } = "refund";

        public string vnp_TmnCode { get; set; } = string.Empty;
        public decimal vnp_Amount { get; set; }
        public DateTime vnp_CreateDate { get; set; }
        public DateTime vnp_TransactionDate { get; set; }
        public string vnp_IpAddr { get; set; }
        public int vnp_TxnRef { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_SecureHash { get; set; }
    }
}