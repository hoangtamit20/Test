using System.ComponentModel.DataAnnotations;
using serverapi.Dtos.Promotions;
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
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Details { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? SeoDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? SeoTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? SeoAlias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalPriceDiscount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Thumbnail { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int ProductTranslationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ProductImageDtos> ListProductImage { get; set; } = new List<ProductImageDtos>();
        /// <summary>
        /// 
        /// </summary>
        public List<PromotionDto>? ListDiscount { get; set; } = new List<PromotionDto>();
    }
}