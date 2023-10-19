namespace serverapi.Dtos.Statisticals
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductItemWithSuggestionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PromotionName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int QuantitySaledInCurrentMonth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Suggestion { get; set; } = string.Empty;
    }
}