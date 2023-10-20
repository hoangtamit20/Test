namespace serverapi.Dtos.Products
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
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
        public bool? IsFeatured { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

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
        public string? Thumbnail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProductTranslationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageId { get; set; } = null!;
    }
}