using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("ProductTranslation")]
public partial class ProductTranslation
{
    [Key]
    public int Id { get; set; }

    [StringLength(60)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? Details { get; set; }

    [StringLength(500)]
    public string? SeoDescription { get; set; }

    [StringLength(200)]
    public string? SeoTitle { get; set; }

    [StringLength(255)]
    public string? SeoAlias { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string LanguageId { get; set; } = null!;

    public int ProductId { get; set; }

    [ForeignKey("LanguageId")]
    [InverseProperty("ProductTranslations")]
    public virtual Language Language { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductTranslations")]
    public virtual Product Product { get; set; } = null!;
}
