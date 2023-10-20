namespace serverapi.Dtos.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerInfoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public required string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? ImagePath { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public DateTime DateCreate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TotalOrder { get; set; }
    }
}