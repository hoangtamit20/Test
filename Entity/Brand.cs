using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity
{
    [Table("Brand")]
    [Index("Name", Name = "UQ__Brand__737584F6F8896A38", IsUnique = true)]
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [InverseProperty("IdBrandNavigation")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}