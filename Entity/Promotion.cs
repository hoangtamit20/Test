using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serverapi.Entity;

/// <summary>
/// 
/// </summary>
[Table("Promotion")]
public partial class Promotion
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
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime FromDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime ToDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool ApplyForAll { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? DiscountPercent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? DiscountAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(4000)]
    public string? ProductIds { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(4000)]
    public string? ProductCategoryIds { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StringLength(50)]
    public string Status { get; set; } = null!;
}