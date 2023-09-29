using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using serverapi.Common;
using serverapi.Enum;

namespace serverapi.Entity;

[Table("PaymentDestination")]
public partial class PaymentDestination : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string? DesLogo { get; set; }

    [StringLength(50)]
    public string? DesShortName { get; set; }

    [StringLength(255)]
    public string? DesName { get; set; }

    public int? DesSortIndex { get; set; }

    public int? ParentId { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<PaymentDestination> InverseParent { get; set; } = new List<PaymentDestination>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual PaymentDestination? Parent { get; set; }

    [InverseProperty("PaymentDestination")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
