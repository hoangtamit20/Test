using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseAuditableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string? CreateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Column(TypeName = "datetime")]
        public DateTime? CreateAt { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [StringLength(50)]
        public string? LastUpdateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdateAt { get; set; }
    }
}