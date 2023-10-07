using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using serverapi.Enum;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Order")]
public partial class Order
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(150)]
    public string ShipName { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string ShipAddress { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
    public string? ShipEmail { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(11)]
    [Unicode(false)]
    public string ShipPhoneNumber { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal TotalPrice { get; set; } = 0;

    /// <summary>
    /// 
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(450)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Order")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual AppUser User { get; set; } = null!;
}