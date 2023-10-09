using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [PrimaryKey("PromotionId", "ProductId")]
    [Table("PromotionProduct")]
    public class PromotionProduct
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
        public int ProductId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("PromotionId")]
        [InverseProperty("PromotionProducts")]
        public virtual Promotion? Promotion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("ProductId")]
        [InverseProperty("PromotionProducts")]
        public virtual Product? Product { get; set; }
    }
}