namespace serverapi.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResultWithData<T> : BaseResult
    {
        /// <summary>
        /// Response data to client
        /// </summary>
        public T? Data { get; set; }
    }
}