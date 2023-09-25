using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity
{
    [Table("Product")]
    [Index("Name", Name = "UQ__Product__737584F6C7073E51", IsUnique = true)]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(19, 0)")]
        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public int IdBrand { get; set; }

        public int IdCategory { get; set; }

        [ForeignKey("IdBrand")]
        [InverseProperty("Products")]
        public virtual Brand IdBrandNavigation { get; set; } = null!;

        [ForeignKey("IdCategory")]
        [InverseProperty("Products")]
        public virtual Category IdCategoryNavigation { get; set; } = null!;

        [InverseProperty("IdProductNavigation")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}