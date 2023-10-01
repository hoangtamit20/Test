using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("CategoryTranslation")]
public partial class CategoryTranslation
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? SeoDescription { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? SeoTitle { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(150)]
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
    public int CategoryId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("CategoryId")]
    [InverseProperty("CategoryTranslations")]
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("LanguageId")]
    [InverseProperty("CategoryTranslations")]
    public virtual Language Language { get; set; } = null!;
}