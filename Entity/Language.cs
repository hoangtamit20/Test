using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Language")]
public partial class Language
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(150)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Language")]
    public virtual ICollection<CategoryTranslation> CategoryTranslations { get; set; } = new List<CategoryTranslation>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Language")]
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();
}
