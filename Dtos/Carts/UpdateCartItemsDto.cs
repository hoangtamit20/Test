namespace serverapi.Dtos.Carts
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateCartItemsDto
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Quantity Of Product
        /// </summary>
        public int Quantity { get; set; }
    }
}