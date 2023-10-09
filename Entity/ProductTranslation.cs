using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("ProductTranslation")]
public partial class ProductTranslation
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(60)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? Details { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(500)]
    public string? SeoDescription { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(200)]
    public string? SeoTitle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? SeoAlias { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string LanguageId { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("LanguageId")]
    [InverseProperty("ProductTranslations")]
    public virtual Language Language { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("ProductId")]
    [InverseProperty("ProductTranslations")]
    public virtual Product Product { get; set; } = null!;
}