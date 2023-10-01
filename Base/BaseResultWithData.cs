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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void Set(bool result, string message, T? data) => (base.Result, base.Message, Data) = (result, message, data);
    }
}