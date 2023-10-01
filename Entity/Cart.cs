using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Cart")]
public partial class Cart
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(450)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("ProductId")]
    [InverseProperty("Carts")]
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("UserId")]
    [InverseProperty("Carts")]
    public virtual AppUser User { get; set; } = null!;
}