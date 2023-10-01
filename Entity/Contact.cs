using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Contact")]
public partial class Contact
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(150)]
    public string? Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(11)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [StringLength(255)]
    public string? Message { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string Status { get; set; } = null!;
}
