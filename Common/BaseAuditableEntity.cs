using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Common
{
    public class BaseAuditableEntity
    {
        [StringLength(50)]
        public string? CreateBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateAt { get; set; }

        [StringLength(50)]
        public string? LastUpdateBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdateAt { get; set; }
    }
}