using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("Contact")]
public partial class Contact
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string? Name { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(11)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(255)]
    public string? Message { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;
}
