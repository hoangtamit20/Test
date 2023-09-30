using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("Product")]
public partial class Product
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal OriginalPrice { get; set; }

    public int Stock { get; set; }

    public int ViewCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    public bool? IsFeatured { get; set; }

    public int CategoryId { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();
}
