using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Product")]
public partial class Product
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal OriginalPrice { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; } = DateTime.Now;

    /// <summary>
    /// 
    /// </summary>
    public bool? IsFeatured { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Product")]
    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Product")]
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Product")]
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
}