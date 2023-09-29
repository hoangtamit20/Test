using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("Payment")]
public partial class Payment
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string? Content { get; set; }

    [StringLength(10)]
    public string? Currency { get; set; }

    [StringLength(50)]
    public string? PaymentRefId { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal? RequiredAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; } = DateTime.Now;

    [Column(TypeName = "datetime")]
    public DateTime? ExpireDate { get; set; }

    [StringLength(10)]
    public string? PaymentLanguage { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal? PaidAmount { get; set; }

    [StringLength(20)]
    public string? PaymentStatus { get; set; }

    [StringLength(255)]
    public string? PaymentLastMessage { get; set; }

    public int MerchantId { get; set; }

    public int PaymentDestinationId { get; set; }

    public int PaymentId { get; set; }

    [ForeignKey("MerchantId")]
    [InverseProperty("Payments")]
    public virtual Merchant Merchant { get; set; } = null!;

    [ForeignKey("PaymentDestinationId")]
    [InverseProperty("Payments")]
    public virtual PaymentDestination PaymentDestination { get; set; } = null!;

    [ForeignKey("PaymentId")]
    [InverseProperty("Payments")]
    public virtual Order PaymentNavigation { get; set; } = null!;

    [InverseProperty("Payment")]
    public virtual ICollection<PaymentNotification> PaymentNotifications { get; set; } = new List<PaymentNotification>();

    [InverseProperty("Payment")]
    public virtual ICollection<PaymentSignature> PaymentSignatures { get; set; } = new List<PaymentSignature>();

    [InverseProperty("Payment")]
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
