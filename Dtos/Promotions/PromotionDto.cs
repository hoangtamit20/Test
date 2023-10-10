using serverapi.Enum;

namespace serverapi.Dtos.Promotions
{
    /// <summary>
    /// 
    /// </summary>
    public class PromotionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int PromotionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public required string PromotionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime FromDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ToDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DiscountType DiscountType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal PriceDisounted { get; set; }
    }
}