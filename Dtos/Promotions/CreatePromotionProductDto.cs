namespace serverapi.Dtos.Promotions
{
    /// <summary>
    /// 
    /// </summary>
    public class CreatePromotionProductDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int PromotionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PromotionProductDto> PromotionProductIds { get; set; } = new List<PromotionProductDto>();
    }
}