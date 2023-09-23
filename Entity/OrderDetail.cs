using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace serverapi.Entity
{
    [PrimaryKey("OrderId", "ProductId")]
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderId { get; set; }
        [Key]
        public int ProductId { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public decimal? UnitPrice { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public decimal? Subtotal { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual Order IdOrderNavigation { get; set; } = null!;

        [ForeignKey("ProductId")]
        [InverseProperty("OrderDetails")]
        public virtual Product IdProductNavigation { get; set; } = null!;
    }
}