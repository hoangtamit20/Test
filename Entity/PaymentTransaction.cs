using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("PaymentTransaction")]
public partial class PaymentTransaction
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
    public string? TranMessage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(500)]
    public string? TranPayload { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(10)]
    public TransactionStatus TranStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "decimal(19, 2)")]
    public decimal? TranAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? TranDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int PaymentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("PaymentId")]
    [InverseProperty("PaymentTransactions")]
    public virtual Payment Payment { get; set; } = null!;
}