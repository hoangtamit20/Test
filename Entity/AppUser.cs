using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace serverapi.Entity;


public partial class AppUser : IdentityUser
{
    

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateOfBirth { get; set; }

    public string? ImageUrl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
