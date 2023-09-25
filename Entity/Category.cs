using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity
{
    [Table("Category")]
    [Index("Name", Name = "UQ__Category__737584F63F07FE8F", IsUnique = true)]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [InverseProperty("IdCategoryNavigation")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}