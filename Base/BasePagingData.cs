namespace serverapi.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasePagingData<T> : BaseResultWithData<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalPage { get; set; }
    }
}