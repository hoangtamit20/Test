using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("PaymentSignature")]
public partial class PaymentSignature
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string? SignValue { get; set; }

    [StringLength(50)]
    public string? SignAlgo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SignDate { get; set; }

    [StringLength(60)]
    public string? SignOwn { get; set; }

    public bool IsValid { get; set; }

    public int PaymentId { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("PaymentSignatures")]
    public virtual Payment Payment { get; set; } = null!;
}
