using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("PaymentNotification")]
public partial class PaymentNotification
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? PaymentRefId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationContent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationMessage { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationSignature { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? NotificationResponseDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int PaymentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey("PaymentId")]
    [InverseProperty("PaymentNotifications")]
    public virtual Payment Payment { get; set; } = null!;
}