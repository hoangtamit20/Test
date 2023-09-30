using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("PaymentTransaction")]
public partial class PaymentTransaction
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string? TranMessage { get; set; }

    [StringLength(500)]
    public string? TranPayload { get; set; }

    [StringLength(10)]
    public TransactionStatus TranStatus { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal? TranAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TranDate { get; set; }

    public int PaymentId { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("PaymentTransactions")]
    public virtual Payment Payment { get; set; } = null!;
}
