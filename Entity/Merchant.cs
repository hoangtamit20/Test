using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Merchant")]
public partial class Merchant
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
    public string? MerchantName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? MerchantWebLink { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? MerchantIpnUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? MerchantReturnUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string? SecretKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [InverseProperty("Merchant")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
