using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using serverapi.Entity;
using serverapi.Models.Orders;
using serverapi.Models.Payments;
using serverapi.Repository.OrderRepository;
using serverapi.Repository.PaymentRepository;

namespace serverapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IOrderRepository orderRepository, IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
        }

        [HttpPost]
        [Route("api/v1/payments")]
        public async Task<ActionResult<Payment>> CreatePayment(PaymentRequest paymentRequest)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the order
            Order order = await _orderRepository.GetOrderByIdAsync(paymentRequest.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            // Create a new payment
            Payment payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.OrderDetails!.AsEnumerable().Sum(od => od.Quantity * od.UnitPrice),
                PaymentMethod = paymentRequest.PaymentMethod,
                Status = PaymentStatus.Pending.ToString(),
            };

            // Save the payment
            await _paymentRepository.CreatePaymentAsync(payment);

            // Update the order status
            order.Status = OrderStatus.Processing.ToString();
            await _orderRepository.UpdateOrderAsync(order);

            // Return the payment
            return Ok(payment);
        }


        [HttpGet]
        [Route("api/v1/payments/{paymentId}/verify")]
        public async Task<ActionResult<Payment>> VerifyPayment(int paymentId)
        {
            // Get the payment
            Payment payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return NotFound();
            }

            // Verify the payment
            var paymentStatus = payment.Status;
            if (paymentStatus == PaymentStatus.Completed.ToString())
            {
                // Update the order status
                payment.Status = PaymentStatus.Completed.ToString();
                await _paymentRepository.UpdatePaymentAsync(payment);

                // Update the order status
                var order = await _orderRepository.GetOrderByIdAsync(payment.OrderId);
                order.Status = OrderStatus.Completed.ToString();
                await _orderRepository.UpdateOrderAsync(order);
            }

            // Return the payment
            return Ok(payment);
        }
    }
}