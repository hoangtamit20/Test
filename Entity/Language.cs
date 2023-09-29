using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("Language")]
public partial class Language
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    public bool IsDefault { get; set; }

    [InverseProperty("Language")]
    public virtual ICollection<CategoryTranslation> CategoryTranslations { get; set; } = new List<CategoryTranslation>();

    [InverseProperty("Language")]
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();
}
