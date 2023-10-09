namespace serverapi.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class PagingFilterDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; } = 10;


        /// <summary>
        /// 
        /// </summary>
        public int? CategoryId { get; set; }


    }
}