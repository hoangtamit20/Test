using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Entity;

namespace serverapi.Repository.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetPaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(int id);
        Task CreatePaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(int id);
    }
}