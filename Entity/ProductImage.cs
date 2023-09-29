using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("ProductImage")]
public partial class ProductImage
{
    [Key]
    public int Id { get; set; }

    [StringLength(4000)]
    public string? ImagePath { get; set; }

    [StringLength(255)]
    public string? Caption { get; set; }

    public int IsDefault { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    public int SortOrder { get; set; }

    public int FileSize { get; set; }

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; } = null!;
}
