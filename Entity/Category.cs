using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using serverapi.Enum;

namespace serverapi.Entity;

[Table("Category")]
public partial class Category
{
    [Key]
    public int Id { get; set; }

    public int SortOrder { get; set; }

    public bool IsShowHome { get; set; }

    public int? ParentId { get; set; }

    // [StringLength(20)]
    public Status Status { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<CategoryTranslation> CategoryTranslations { get; set; } = new List<CategoryTranslation>();

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
