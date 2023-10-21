using System.ComponentModel.DataAnnotations.Schema;

namespace serverapi.Dtos.Promotions
{
    /// <summary>
    /// 
    /// </summary>
    public class PromotionInfoDto
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
        public string DiscountType { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [Column(TypeName = "decimal(19, 2)")]
        public decimal DiscountValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PromotionCategoryDto> ListPromotionCategory { get; set; } = new List<PromotionCategoryDto>();
        /// <summary>
        /// 
        /// </summary>
        public List<PromotionProductDto> ListPromotionProduct { get; set; } = new List<PromotionProductDto>();
    }
}