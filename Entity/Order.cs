using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using serverapi.Enum;

namespace serverapi.Entity;

[Table("Order")]
public partial class Order
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [StringLength(150)]
    public string ShipName { get; set; } = null!;

    [StringLength(255)]
    public string ShipAddress { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? ShipEmail { get; set; }

    [StringLength(11)]
    [Unicode(false)]
    public string ShipPhoneNumber { get; set; } = null!;

    // [StringLength(50)]
    public OrderStatus Status { get; set; }

    [StringLength(450)]
    public string UserId { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("PaymentNavigation")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual AppUser User { get; set; } = null!;
}
