using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    [ForeignKey("UserId")]
    [InverseProperty("Carts")]
    public virtual AppUser User { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Cart")]
    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

}