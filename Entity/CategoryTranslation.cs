using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("CategoryTranslation")]
public partial class CategoryTranslation
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? SeoDescription { get; set; }

    [StringLength(255)]
    public string? SeoTitle { get; set; }

    [StringLength(150)]
    public string? SeoAlias { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string LanguageId { get; set; } = null!;

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("CategoryTranslations")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("LanguageId")]
    [InverseProperty("CategoryTranslations")]
    public virtual Language Language { get; set; } = null!;
}
