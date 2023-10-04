using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Entity;
using serverapi.Repository.OrderRepository;

namespace serverapi.Repository.OrderDetailRepository
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailRepository : IOrderDetailRepository
    {

        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public OrderDetailRepository(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync()
        {
            return await _context.OrderDetails!.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>

        public async Task<OrderDetail> GetOrderDetailByIdAsync(int orderId, int productId)
        {
            return (await _context.OrderDetails!.FindAsync(orderId, productId))!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>

        public async Task CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            // // Tạo một instance của ValidationContext
            // var context = new ValidationContext(orderDetail, serviceProvider: null, items: null);

            // // Tạo một danh sách để lưu trữ các kết quả validation
            // var results = new List<ValidationResult>();

            // // Thực hiện validation và lưu kết quả vào danh sách
            // var isValid = Validator.TryValidateObject(orderDetail, context, results, true);

            // // Nếu validation không thành công, ném một ngoại lệ với thông tin về các lỗi
            // if (!isValid)
            // {
            //     var errors = results.Select(r => r.ErrorMessage);
            //     throw new Exception("Validation failed: " + string.Join(", ", errors));
            // }

            // Nếu validation thành công, tiếp tục thực hiện các hành động của repository
            _context.OrderDetails!.Add(orderDetail);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>

        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.Entry(orderDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>

        public async Task DeleteOrderDetailAsync(int orderId, int productId)
        {
            var orderDetail = await _context.OrderDetails!.FindAsync(orderId, productId);
            _context.OrderDetails.Remove(orderDetail!);
            await _context.SaveChangesAsync();
        }
    }
}