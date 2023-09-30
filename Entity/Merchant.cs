using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("Merchant")]
public partial class Merchant
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? MerchantName { get; set; }

    [StringLength(255)]
    public string? MerchantWebLink { get; set; }

    [StringLength(255)]
    public string? MerchantIpnUrl { get; set; }

    [StringLength(255)]
    public string? MerchantReturnUrl { get; set; }

    [StringLength(50)]
    public string? SecretKey { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Merchant")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
