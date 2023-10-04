using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Entity;

namespace serverapi.Repository.OrderRepository
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public OrderRepository(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders!.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return (await _context.Orders!.FindAsync(id))!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>

        public async Task CreateOrderAsync(Order order)
        {
            _context.Orders!.Add(order);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders!.FindAsync(id);
            _context.Orders.Remove(order!);
            await _context.SaveChangesAsync();
        }
    }
}