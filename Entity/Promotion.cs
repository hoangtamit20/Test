using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity;

[Table("Promotion")]
public partial class Promotion
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime FromDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ToDate { get; set; }

    public bool ApplyForAll { get; set; }

    public int? DiscountPercent { get; set; }

    public int? DiscountAmount { get; set; }

    [StringLength(4000)]
    public string? ProductIds { get; set; }

    [StringLength(4000)]
    public string? ProductCategoryIds { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;
}
