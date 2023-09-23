using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using PetShop.Entity;

namespace serverapi.Entity
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public decimal? ShippingCost { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdateAt { get; set; }

        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("Orders")]
        public virtual NguoiDung UserIdNavigation { get; set; } = null!;

        [InverseProperty("IdOrderNavigation")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [InverseProperty("IdOrderNavigation")]
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}