namespace serverapi.Dtos.Promotions
{
    /// <summary>
    /// 
    /// </summary>
    public class CreatePromotionCategoryDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int PromotionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PromotionCategoryDto> PromotionCategoryIds { get; set; } = new List<PromotionCategoryDto>();
    }
}