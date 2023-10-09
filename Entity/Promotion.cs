using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using serverapi.Enum;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Promotion")]
public partial class Promotion
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(200)]
    public required string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>

    [Column(TypeName = "datetime")]
    public DateTime FromDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime ToDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool ApplyForAll { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DiscountType DiscountType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Status Status { get; set; }

    // Use many-to-many relationship with Product
    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Promotion")]
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();

    // Use many-to-many relationship with Category
    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Promotion")]
    public virtual ICollection<PromotionCategory> PromotionCategories { get; set; } = new List<PromotionCategory>();
}