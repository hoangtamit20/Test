using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serverapi.Entity;

[Table("Cart")]
public partial class Cart
{
    [Key]
    public int Id { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [StringLength(450)]
    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Carts")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Carts")]
    public virtual AppUser User { get; set; } = null!;
}
