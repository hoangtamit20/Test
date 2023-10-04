using System.ComponentModel.DataAnnotations;
using serverapi.Entity;

namespace serverapi.Dtos.Products
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductInfoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(60)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// giá gốc
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// số lượng còn trong kho
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set; } = null!;

        /// <summary>
        /// mô tả sản phẩm
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Chi tiết sản phẩm
        /// </summary>
        [StringLength(255)]
        public string? Details { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(500)]
        public string? SeoDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(200)]
        public string? SeoTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(255)]
        public string? SeoAlias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ProductImageDtos> ListProductImage { get; set; } = new List<ProductImageDtos>();
    }
}