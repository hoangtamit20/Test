using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("PaymentNotification")]
public partial class PaymentNotification
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? PaymentRefId { get; set; }

    [StringLength(50)]
    public string? NotificationDate { get; set; }

    [StringLength(50)]
    public string? NotificationAmount { get; set; }

    [StringLength(50)]
    public string? NotificationContent { get; set; }

    [StringLength(50)]
    public string? NotificationMessage { get; set; }

    [StringLength(50)]
    public string? NotificationSignature { get; set; }

    [StringLength(50)]
    public string? NotificationStatus { get; set; }

    [StringLength(50)]
    public string? NotificationResponseDate { get; set; }

    public int PaymentId { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("PaymentNotifications")]
    public virtual Payment Payment { get; set; } = null!;
}
