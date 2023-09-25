// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using FluentValidation;
// using Microsoft.AspNetCore.Mvc;
// using serverapi.Entity;
// using serverapi.Models.Orders;
// using serverapi.Repository.OrderDetailRepository;
// using serverapi.Repository.OrderRepository;

// namespace serverapi.Controllers
// {

//     public static class OrdersEndPoints
//     {
//         public static void MapOrdersEndPoints(this IEndpointRouteBuilder app)
//         {
//             app.MapPost("api/orders", async (OrderRequest orderRequest, IValidator<Order> validator, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository) =>
//             {

//                 // Create a new order
//                 Order order = new Order
//                 {
//                     UserId = orderRequest.UserId!,
//                     ShippingAddress = orderRequest.ShippingAddress,
//                     ShippingCost = orderRequest.ShippingCost,
//                     PaymentMethod = orderRequest.PaymentMethod,
//                     Status = OrderStatus.Pending.ToString(),
//                 };
//                 // Validate the request
//                 var validateResult = await validator.ValidateAsync(order);
//                 if (!validateResult.IsValid)
//                 {
//                     return Results.ValidationProblem(validateResult.Errors);
//                 }

//                 // Save the order
//                 await orderRepository.CreateOrderAsync(order);

//                 // Create order details
//                 foreach (OrderDetailRequest orderDetailRequest in orderRequest.OrderDetails!)
//                 {
//                     OrderDetail orderDetail = new OrderDetail
//                     {
//                         OrderId = order.Id,
//                         ProductId = orderDetailRequest.ProductId,
//                         Quantity = orderDetailRequest.Quantity,
//                         UnitPrice = orderDetailRequest.UnitPrice,
//                         Subtotal = orderDetailRequest.Quantity * orderDetailRequest.UnitPrice,
//                     };

//                     await orderDetailRepository.CreateOrderDetailAsync(orderDetail);
//                 }

//                 // // Save order details
//                 // await _orderDetailRepository.SaveChangesAsync();

//                 // Return the order
//                 return Ok(order);
//             });
//         }

//         private readonly IOrderRepository _orderRepository;
//         private readonly IOrderDetailRepository _orderDetailRepository;
//         public OrdersController(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
//         {
//             _orderRepository = orderRepository;
//             _orderDetailRepository = orderDetailRepository;
//         }

//         [HttpPost]
//         [Route("orders")]
//         public async Task<ActionResult<Order>> CreateOrder(OrderRequest orderRequest)
//         {
//             // Validate the request
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             // Create a new order
//             Order order = new Order
//             {
//                 UserId = orderRequest.UserId!,
//                 ShippingAddress = orderRequest.ShippingAddress,
//                 ShippingCost = orderRequest.ShippingCost,
//                 PaymentMethod = orderRequest.PaymentMethod,
//                 Status = OrderStatus.Pending.ToString(),
//             };

//             // Save the order
//             await _orderRepository.CreateOrderAsync(order);

//             // Create order details
//             foreach (OrderDetailRequest orderDetailRequest in orderRequest.OrderDetails!)
//             {
//                 OrderDetail orderDetail = new OrderDetail
//                 {
//                     OrderId = order.Id,
//                     ProductId = orderDetailRequest.ProductId,
//                     Quantity = orderDetailRequest.Quantity,
//                     UnitPrice = orderDetailRequest.UnitPrice,
//                     Subtotal = orderDetailRequest.Quantity * orderDetailRequest.UnitPrice,
//                 };

//                 await _orderDetailRepository.CreateOrderDetailAsync(orderDetail);
//             }

//             // // Save order details
//             // await _orderDetailRepository.SaveChangesAsync();

//             // Return the order
//             return Ok(order);
//         }
//     }
// }