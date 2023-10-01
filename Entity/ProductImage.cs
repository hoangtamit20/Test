using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("ProductImage")]
public partial class ProductImage
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(4000)]
    public string? ImagePath { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? Caption { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int IsDefault { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int FileSize { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; } = null!;
}