using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [PrimaryKey("PromotionId", "CategoryId")]
    [Table("PromotionCategory")]
    public class PromotionCategory
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int PromotionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("PromotionId")]
        [InverseProperty("PromotionCategories")]
        public virtual Promotion? Promotion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("CategoryId")]
        [InverseProperty("PromotionCategories")]
        public virtual Category? Category { get; set; }
    }
}