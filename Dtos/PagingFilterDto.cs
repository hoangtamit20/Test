using System.ComponentModel;

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
        [DefaultValue(null)]
        public string? Filter { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(null)]
        public int? CategoryId { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(null)]
        public string? LanguageId { get; set; } = null;
    }

}