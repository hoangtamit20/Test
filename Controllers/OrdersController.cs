using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using serverapi.Entity;
using serverapi.Models.Orders;
using serverapi.Repository.OrderDetailRepository;
using serverapi.Repository.OrderRepository;

namespace serverapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        public OrdersController(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        [HttpPost]
        [Route("api/v1/orders")]
        public async Task<ActionResult<Order>> CreateOrder(OrderRequest orderRequest)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new order
            Order order = new Order
            {
                UserId = orderRequest.UserId!,
                ShippingAddress = orderRequest.ShippingAddress,
                ShippingCost = orderRequest.ShippingCost,
                PaymentMethod = orderRequest.PaymentMethod,
                Status = OrderStatus.Pending.ToString(),
            };

            // Save the order
            await _orderRepository.CreateOrderAsync(order);

            // Create order details
            foreach (OrderDetailRequest orderDetailRequest in orderRequest.OrderDetails!)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = orderDetailRequest.ProductId,
                    Quantity = orderDetailRequest.Quantity,
                    UnitPrice = orderDetailRequest.UnitPrice,
                    Subtotal = orderDetailRequest.Quantity * orderDetailRequest.UnitPrice,
                };

                await _orderDetailRepository.CreateOrderDetailAsync(orderDetail);
            }

            // // Save order details
            // await _orderDetailRepository.SaveChangesAsync();

            // Return the order
            return Ok(order);
        }
    }
}