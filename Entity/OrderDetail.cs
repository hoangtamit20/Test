using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[PrimaryKey("OrderId", "ProductId")]
[Table("OrderDetail")]
public partial class OrderDetail
{
    [Key]
    public int OrderId { get; set; }

    [Key]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal SubTotal { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetails")]
    public virtual Product Product { get; set; } = null!;
}
