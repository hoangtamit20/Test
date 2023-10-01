using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using serverapi.Enum;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Category")]
public partial class Category
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsShowHome { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Status Status { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Category")]
    public virtual ICollection<CategoryTranslation> CategoryTranslations { get; set; } = new List<CategoryTranslation>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
