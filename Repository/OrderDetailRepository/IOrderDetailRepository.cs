using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Entity;

namespace serverapi.Repository.OrderDetailRepository
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync();
        Task<OrderDetail> GetOrderDetailByIdAsync(int orderId, int productId);
        Task CreateOrderDetailAsync(OrderDetail orderDetail);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task DeleteOrderDetailAsync(int orderId, int productId);
    }
}