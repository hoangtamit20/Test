using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;
/// <summary>
/// 
/// </summary>
[Index("UserName", Name = "AppUser_UserName", IsUnique = true)]
[Index("Email", Name = "AppUser_Email", IsUnique = true)]
public partial class AppUser : IdentityUser
{
    /// <summary>
    /// 
    /// </summary>
    [StringLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("User")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}