using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Entity;

namespace serverapi.Repository.OrderDetailRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOrderDetailRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<OrderDetail> GetOrderDetailByIdAsync(int orderId, int productId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        Task CreateOrderDetailAsync(OrderDetail orderDetail);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task DeleteOrderDetailAsync(int orderId, int productId);
    }
}