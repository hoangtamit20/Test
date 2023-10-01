using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("PaymentSignature")]
public partial class PaymentSignature
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(500)]
    public string? SignValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? SignAlgo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? SignDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(60)]
    public string? SignOwn { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int PaymentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("PaymentId")]
    [InverseProperty("PaymentSignatures")]
    public virtual Payment Payment { get; set; } = null!;
}