using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using serverapi.Enum;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Payment")]
public partial class Payment
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
    public string? PaymentContent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(10)]
    public string? PaymentCurrency { get; set; }

    // /// <summary>
    // /// 
    // /// </summary>
    // [StringLength(50)]
    // public string? PaymentRefId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal? RequiredAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(10)]
    public string? PaymentLanguage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal? PaidAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(20)]
    public string? PaymentStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? PaymentLastMessage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? LastUpdateAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? LastUpdateBy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MerchantId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int PaymentDestinationId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("MerchantId")]
    [InverseProperty("Payments")]
    public virtual Merchant Merchant { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("PaymentDestinationId")]
    [InverseProperty("Payments")]
    public virtual PaymentDestination PaymentDestination { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order Order { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Payment")]
    public virtual ICollection<PaymentNotification> PaymentNotifications { get; set; } = new List<PaymentNotification>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Payment")]
    public virtual ICollection<PaymentSignature> PaymentSignatures { get; set; } = new List<PaymentSignature>();

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Payment")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}