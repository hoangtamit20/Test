using System.ComponentModel.DataAnnotations.Schema;
using serverapi.Enum;

namespace serverapi.Dtos.Promotions
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdatePromotionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
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
        [Column(TypeName = "decimal(19, 2)")]
        public decimal DiscountValue { get; set; }
    }
}