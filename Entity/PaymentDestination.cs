using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using serverapi.Common;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("PaymentDestination")]
public partial class PaymentDestination : BaseAuditableEntity
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? DesLogo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? DesShortName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? DesName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? DesSortIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Parent")]
    public virtual ICollection<PaymentDestination> InverseParent { get; set; } = new List<PaymentDestination>();

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual PaymentDestination? Parent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("PaymentDestination")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}