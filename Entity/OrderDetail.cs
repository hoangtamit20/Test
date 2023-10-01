using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[PrimaryKey("OrderId", "ProductId")]
[Table("OrderDetail")]
public partial class OrderDetail
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int OrderId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int ProductId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal SubTotal { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public virtual Order Order { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetails")]
    public virtual Product Product { get; set; } = null!;
}