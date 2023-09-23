using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Entity
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public decimal? Amount { get; set; }

        [StringLength(255)]
        public string? PaymentMethod { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("Payments")]
        public virtual Order IdOrderNavigation { get; set; } = null!;
    }
}