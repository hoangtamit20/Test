using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Entity;

namespace serverapi.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PetShopDbContext _context;

        public OrderRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders!.ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return (await _context.Orders!.FindAsync(id))!;
        }

        public async Task CreateOrderAsync(Order order)
        {
            _context.Orders!.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders!.FindAsync(id);
            _context.Orders.Remove(order!);
            await _context.SaveChangesAsync();
        }
    }
}