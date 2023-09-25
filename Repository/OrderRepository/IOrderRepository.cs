using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Entity;

namespace serverapi.Repository.OrderRepository
{
    public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrdersAsync();
    Task<Order> GetOrderByIdAsync(int id);
    Task CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(int id);
}
}