using serverapi.Dtos;

namespace serverapi.Services.PagingAndFilterService
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingFilterService<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pagingFilterDto"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public List<T> FilterAndPage(List<T> list, PagingFilterDto pagingFilterDto, Func<T, bool> filter, Func<T, object> orderBy)
        {
            if (pagingFilterDto.Filter is not null)
            {
                list = list.Where(filter).ToList();
            }

            list = list.Skip(pagingFilterDto.PageSize * (pagingFilterDto.PageIndex - 1))
                       .Take(pagingFilterDto.PageSize)
                       .OrderBy(orderBy)
                       .ToList();

            return list;
        }
    }

}