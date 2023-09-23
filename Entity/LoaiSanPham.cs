using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Entity
{
    public class LoaiSanPham
    {
        public int IdLoaiSanPham { get; set; }
        [StringLength(150)]
        [Column("NVARCHAR")]
        public string? TenSanPham { get; set; }
    }
}