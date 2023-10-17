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
        /// <param name="categoryId"></param>
        /// <param name="TotalPagge"></param>
        /// <returns></returns>
        public List<T> FilterAndPage(
            List<T> list,
            PagingFilterDto pagingFilterDto,
            Func<T, bool> filter,
            Func<T, bool> categoryId,
            Func<T, object> orderBy,
            ref int TotalPagge
        )
        {
            if (pagingFilterDto.Filter is not null)
            {
                list = list.Where(filter).ToList();
            }

            if (pagingFilterDto.PageSize == 0 || pagingFilterDto.PageIndex == 0)
            {
                list = list.OrderBy(orderBy)
                           .ToList();
            }

            if (pagingFilterDto.CategoryId is not null)
            {
                list.Where(categoryId);
            }

            TotalPagge = (int)Math.Ceiling((double)list.Count / pagingFilterDto.PageSize) == 0 ? 1 : (int)Math.Ceiling((double)list.Count / pagingFilterDto.PageSize);
            System.Console.WriteLine(TotalPagge);

            list = list.Skip(pagingFilterDto.PageSize * (pagingFilterDto.PageIndex - 1))
                       .Take(pagingFilterDto.PageSize)
                       .OrderBy(orderBy)
                       .ToList();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pagingFilterDto"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="TotalPagge"></param>
        /// <returns></returns>

        public List<T> FilterAndPage(
            List<T> list,
            PagingFilterDto pagingFilterDto,
            Func<T, bool> filter,
            Func<T, object> orderBy,
            ref int TotalPagge
        )
        {
            if (pagingFilterDto.Filter is not null)
            {
                list = list.Where(filter).ToList();
            }
            TotalPagge = (int)Math.Ceiling((double)list.Count / pagingFilterDto.PageSize) == 0 ? 1 : (int)Math.Ceiling((double)list.Count / pagingFilterDto.PageSize);
            System.Console.WriteLine(TotalPagge);
            if (pagingFilterDto.PageSize == 0 || pagingFilterDto.PageIndex == 0)
            {
                list = list.OrderBy(orderBy)
                           .ToList();
            }
            else
            {
                list = list.Skip(pagingFilterDto.PageSize * (pagingFilterDto.PageIndex - 1))
                           .Take(pagingFilterDto.PageSize)
                           .OrderBy(orderBy)
                           .ToList();
            }
            return list;
        }
    }

}