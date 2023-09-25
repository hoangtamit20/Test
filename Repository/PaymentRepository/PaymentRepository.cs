using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Entity;

namespace serverapi.Repository.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {

        private readonly PetShopDbContext _context;

        public PaymentRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync()
        {
            return await _context.Payments!.ToListAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return (await _context.Payments!.FindAsync(id))!;
        }

        public async Task CreatePaymentAsync(Payment payment)
        {
            // // Tạo một instance của ValidationContext
            // var context = new ValidationContext(payment, serviceProvider: null, items: null);

            // // Tạo một danh sách để lưu trữ các kết quả validation
            // var results = new List<ValidationResult>();

            // // Thực hiện validation và lưu kết quả vào danh sách
            // var isValid = Validator.TryValidateObject(payment, context, results, true);

            // // Nếu validation không thành công, ném một ngoại lệ với thông tin về các lỗi
            // if (!isValid)
            // {
            //     var errors = results.Select(r => r.ErrorMessage);
            //     throw new Exception("Validation failed: " + string.Join(", ", errors));
            // }

            // Nếu validation thành công, tiếp tục thực hiện các hành động của repository
            _context.Payments!.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments!.FindAsync(id);
            _context.Payments.Remove(payment!);
            await _context.SaveChangesAsync();
        }
    }
}