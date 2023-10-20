using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Constants;
using serverapi.Dtos;
using serverapi.Dtos.Customers;
using serverapi.Dtos.OrderDetails;
using serverapi.Dtos.Orders;
using serverapi.Dtos.Payments;
using serverapi.Dtos.Products;
using serverapi.Entity;
using serverapi.Enum;
using serverapi.Libraries.SignalRs;
using serverapi.Services.PagingAndFilterService;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<NotificationHub> _hubContext;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hubContext"></param>
        /// <param name="emailSender"></param>
        public AdminController(
            PetShopDbContext context,
            IHubContext<NotificationHub> hubContext,
            IEmailSender emailSender)
        {
            _context = context;
            _hubContext = hubContext;
            _emailSender = emailSender;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestSignalR()
        {
            await _hubContext.Clients.All.SendAsync(SignalRConstant.ReceiveNotification, $"Message send from SignalR");
            return Ok();
        }

        /// <summary>
        /// Accept order (Admin)
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("accept-order/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptOrderOfCustomer(int orderId)
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Order is null!" } });
            var orderExists = await _context.Orders.FindAsync(orderId);
            if (orderExists is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Cannot found order {orderId}" } });
            if (orderExists.Status != OrderStatus.Confirmed)
                return BadRequest(new BaseBadRequestResult()
                { Errors = new List<string>() { $"Cannot accept order with id {orderId} because the order maybe was payment or not yet payment" } });
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                // change order status
                orderExists.Status = OrderStatus.Success;
                _context.Entry<Order>(orderExists).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderExists.UserId);
                    await _emailSender.SendEmailAsync(user?.Email!, $"Accept Order is Successfull!", $"Congratulations {user?.Name}! Your order has been completed!");
                    await _transaction.CommitAsync();
                    return Ok(new BaseResultSuccess() { Message = $"Accept order {orderId} success!" });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
        }

        /// <summary>
        /// Cancel order (Admin)
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cancel-order/{orderId}")]
        public async Task<IActionResult> CancelOrderOfCustomer(int orderId)
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Order is null!" } });
            var orderExists = await _context.Orders.FindAsync(orderId);
            if (orderExists is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Cannot found order {orderId}" } });
            if (orderExists.Status == OrderStatus.Success)
                return BadRequest(new BaseBadRequestResult()
                { Errors = new List<string>() { $"Cannot cancel order with id {orderId} because the order was shipped." } });
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                //update order status
                orderExists.Status = OrderStatus.Canceled;
                _context.Entry<Order>(orderExists).State = EntityState.Modified;
                //refund product stock
                var listOrderDetail = await _context.OrderDetails
                    .Where(odl => odl.OrderId == orderId)
                    .ToListAsync();
                if (listOrderDetail is not null)
                {
                    listOrderDetail.ForEach(odl =>
                    {
                        var product = _context.Products.Find(odl.ProductId);
                        if (product is not null)
                        {
                            // plus quantity to product stock
                            product.Stock += odl.Quantity;
                            _context.Entry<Product>(product).State = EntityState.Modified;
                        }
                    });
                }
                //TODO : Refund payment for customer
                //todo here
                //

                try
                {
                    await _context.SaveChangesAsync();
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderExists.UserId);
                    await _emailSender.SendEmailAsync(user?.Email!, $"Cancel Order!", $"Sincerely sorry {user?.Name}! Your order has been cancelled.");
                    await _transaction.CommitAsync();
                    return Ok(new BaseResultSuccess() { Message = $"Cancel order {orderId} success" });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
        }

        /// <summary>
        /// Create product, producttranslation, productimages
        /// </summary>
        /// <param name="createProductDto"> Inlcude information of product, producttranslation, and List string of ProductImages for ImagePath </param>
        /// <returns></returns>
        [HttpPost]
        [Route("create-product-admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResultWithData<CreateProductDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (ModelState.IsValid)
            {
                if (
                    _context.Products is null
                    || _context.ProductImages is null
                    || _context.ProductTranslations is null
                    || _context.Languages is null
                )
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Error! Db is null." } });
                using (var _transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        createProductDto.LanguageId = createProductDto.LanguageId ?? "VN";
                        var product = createProductDto.Adapt<Product>();
                        _context.Products.Add(product);
                        await _context.SaveChangesAsync();
                        var productTranslation = createProductDto.Adapt<ProductTranslation>();
                        productTranslation.ProductId = product.Id;
                        _context.ProductTranslations.Add(productTranslation);
                        var productImages = new List<ProductImage>();
                        if (createProductDto.ImageUrls is not null && createProductDto.ImageUrls.Count > 0)
                            createProductDto.ImageUrls.ForEach(image =>
                            {
                                productImages.Add(new ProductImage() { ProductId = product.Id, ImagePath = image });
                            });
                        _context.ProductImages.AddRange(productImages);
                        await _context.SaveChangesAsync();
                        _transaction.Commit();
                        return Ok(new BaseResultWithData<CreateProductDto>()
                        {
                            Success = true,
                            Message = "Create product is success.",
                            Data = createProductDto
                        });
                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                    }
                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList() });
        }

        /// <summary>
        /// update product, producttranslation
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updateProductDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update-product-admin/{productId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDto updateProductDto)
        {
            if (ModelState.IsValid)
            {
                if (productId != updateProductDto.Id)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Route id : {productId} not equal to ProductId {updateProductDto.Id}" } });
                var productExsits = await _context.Products.FindAsync(updateProductDto.Id);
                if (productExsits is null)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with Id {updateProductDto.Id} not found!" } });
                var productTransExists = await _context.ProductTranslations
                    .FirstOrDefaultAsync(pt =>
                        pt.ProductId == productExsits.Id
                        && pt.LanguageId == updateProductDto.LanguageId);
                if (productTransExists is null)
                {
                    var proTrans = updateProductDto.Adapt<ProductTranslation>();
                    proTrans.ProductId = productExsits.Id;
                    _context.ProductTranslations.Add(proTrans);
                }
                else
                {
                    productTransExists = updateProductDto.Adapt<ProductTranslation>();
                    _context.Entry<ProductTranslation>(productTransExists).State = EntityState.Modified;
                }
                productExsits = updateProductDto.Adapt<Product>();
                _context.Entry<Product>(productExsits).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList() });
        }


        /// <summary>
        /// Delete Product by Id (Admin)
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns></returns>

        [HttpDelete]
        [Route("delete-product-admin{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { "Internal server error" } });
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with id : {id} not found" } });
            }
            try
            {
                _context.Products.Remove(product);
                var productTransRemove = await _context.ProductTranslations.FirstOrDefaultAsync(p => p.ProductId == id);
                if (productTransRemove is not null)
                {
                    _context.ProductTranslations.Remove(productTransRemove);
                    using (var _transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            await _context.SaveChangesAsync();
                            await _transaction.CommitAsync();
                            return NoContent();
                        }
                        catch (Exception ex)
                        {
                            await _transaction.RollbackAsync();
                            return StatusCode(500, new BaseBadRequestResult()
                            {
                                Errors = new List<string>()
                            {
                                "An error occurred while performing the transaction",
                                $"Không nên hiển thị lỗi này cho client - {ex.Message}"
                            }
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult()
                {
                    Errors = new List<string>()
                {
                    "Internal server error",
                    $"Không nên hiển thị lỗi này cho client - {ex.Message}"
                }
                });
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list-order-confirmed")]
        [ProducesResponseType(typeof(BaseResultWithData<List<OrderConfirmedDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetListOrderConfirmed()
        {
            if (_context.Orders is null || _context.Users is null)
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Orders or Users is null!" } });
            }
            var orders = await _context.Orders
                .Include(od => od.User)
                .Where(od => od.Status == OrderStatus.Confirmed)
                .Select(od => new OrderConfirmedDto()
                {
                    Id = od.Id,
                    OrderDate = od.OrderDate,
                    Status = od.Status,
                    ShipAddress = od.ShipAddress,
                    ShipEmail = od.ShipEmail,
                    ShipName = od.ShipName,
                    ShipPhoneNumber = od.ShipPhoneNumber,
                    TotalPrice = od.TotalPrice,
                    Name = od.User.Name,
                    Email = od.User.Email!
                })
                .ToListAsync();
            return Ok(new BaseResultWithData<List<OrderConfirmedDto>>()
            {
                Success = true,
                Message = "List orders were confirmed",
                Data = orders
            });
        }

        /// <summary>
        /// This API endpoint retrieves a list of orders with optional filtering and paging.
        /// </summary>
        /// <param name="pagingFilterDto">An object containing the paging and filtering options.</param>
        /// <returns>
        /// A list of <see cref="OrderInfoAdminDto"/> objects, each representing the details of an order.
        /// Each object includes the order ID, order date, customer email, customer name, order status, total price, ship name, ship address, ship email, and ship phone number.
        /// The list is optionally filtered and paged based on the provided <see cref="PagingFilterDto"/>.
        /// </returns>
        /// <response code="200">Returns the list of orders.</response>
        /// <remarks>
        ///     GET:
        /// {
        ///     * Notice : IdCategory is OrderStatus (InProgress : 0; Confirmed = 1; Shipping = 2; Success = 3; Canceled = 4)
        /// }
        /// </remarks>
        [HttpGet]
        [Route("list-order-admin")]
        [ProducesResponseType(typeof(BasePagingData<List<OrderInfoAdminDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersWithFilterAndPaging([FromQuery] PagingFilterDto pagingFilterDto)
        {
            var listOrders = await _context.Orders
                .Include(o => o.User)
                .Select(o => new OrderInfoAdminDto
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    CustomerEmail = o.User.Email,
                    CustomerName = o.User.Name,
                    OrderStatus = o.Status,
                    TotalPrice = o.TotalPrice,
                    ShipName = o.ShipName,
                    ShipAddress = o.ShipAddress,
                    ShipEmail = o.ShipEmail,
                    ShipPhoneNumber = o.ShipPhoneNumber
                })
                .ToListAsync();
            int TotalPage = 0;
            var _pagingFilterService = new PagingFilterService<OrderInfoAdminDto>();
            listOrders = _pagingFilterService.FilterAndPage(
                listOrders,
                pagingFilterDto,
                o => o.ToString().Contains(pagingFilterDto.Filter!, StringComparison.OrdinalIgnoreCase),
                o => (int)o.OrderStatus == pagingFilterDto.CategoryId,
                o => o.OrderDate,
                ref TotalPage
            );
            return Ok(new BasePagingData<List<OrderInfoAdminDto>>()
            {
                TotalPage = TotalPage,
                Success = true,
                Message = "Orders for admin",
                Data = listOrders
            });
        }



        /// <summary>
        /// This API endpoint retrieves the details of an order by its ID.
        /// </summary>
        /// <param name="language">The language in which the product names should be returned. Defaults to "VN" if not provided.</param>
        /// <param name="orderId">The ID of the order for which details are required.</param>
        /// <returns>
        /// A list of <see cref="OrderDetailByOrderIdDto"/> objects, each representing the details of a product in the order.
        /// Each object includes the product name (in the specified language), quantity, price, and subtotal.
        /// </returns>
        /// <response code="200">Returns the order details for the specified order ID.</response>
        /// <response code="400">If the request is invalid, a list of error messages is returned.</response>
        [HttpGet]
        [Route("orderdetails-by-orderid/{orderId}")]
        [ProducesResponseType(typeof(BaseResultWithData<List<OrderDetailByOrderIdDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> OrderDetailsByOrderId(string language, int orderId)
        {
            if (ModelState.IsValid)
            {
                language = language ?? "VN";
                var orderDetails = await _context.OrderDetails
                    .Include(odl => odl.Product)
                    .ThenInclude(p => p.ProductTranslations)
                    .Where(odl => odl.OrderId == orderId)
                    .Select(odl => new OrderDetailByOrderIdDto
                    {
                        ProductName = odl.Product.ProductTranslations.FirstOrDefault(pp => pp.ProductId == odl.ProductId)!.Name,
                        ProductQuantity = odl.Quantity,
                        ProductPrice = odl.SubTotal / odl.Quantity,
                        SubTotal = odl.SubTotal
                    })
                    .ToListAsync();
                return Ok(new BaseResultWithData<List<OrderDetailByOrderIdDto>>()
                {
                    Success = true,
                    Message = $"List Order details by order with {orderId}",
                    Data = orderDetails
                });
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        /// <summary>
        /// This API endpoint retrieves the payment information for an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order for which payment information is required.</param>
        /// <returns>
        /// A list of <see cref="PaymentInfoByOrderIdDto"/> objects, each representing the payment information for the specified order.
        /// Each object includes the payment ID, payment content, payment date, payment method, and total amount.
        /// </returns>
        /// <response code="200">Returns the payment information for the specified order ID.</response>
        /// <response code="400">If the request is invalid, a list of error messages is returned.</response>
        [HttpGet]
        [Route("payment-info-by-orderid/{orderId}")]
        public async Task<IActionResult> GetPaymentInfoByOrderId(int orderId)
        {
            if (ModelState.IsValid)
            {
                var payments = await _context.Payments
                    .Include(p => p.PaymentDestination)
                    .Where(p => p.OrderId == orderId)
                    .Select(p => new PaymentInfoByOrderIdDto
                    {
                        PaymentId = p.Id,
                        PaymentContent = p.PaymentContent,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentDestination.DesShortName ?? string.Empty,
                        TotalAmount = p.RequiredAmount ?? 0
                    })
                    .ToListAsync();
                return Ok(new BaseResultWithData<List<PaymentInfoByOrderIdDto>>()
                {
                    Success = true,
                    Message = $"Payments by order id {orderId}",
                    Data = payments
                });
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        /// <summary>
        /// This API endpoint retrieves a list of customers with optional filtering and paging.
        /// </summary>
        /// <param name="pagingFilterDto">An object containing the paging and filtering options.</param>
        /// <returns>
        /// A list of <see cref="CustomerInfoDto"/> objects, each representing the details of a customer.
        /// Each object includes the user ID, name, image path, creation date, and total number of orders.
        /// The list is optionally filtered and paged based on the provided <see cref="PagingFilterDto"/>.
        /// </returns>
        /// <response code="200">Returns the list of customers.</response>
        [HttpGet]
        [Route("list-customer")]
        [ProducesResponseType(typeof(BasePagingData<List<CustomerInfoDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CustomerInfos([FromQuery] PagingFilterDto pagingFilterDto)
        {
            var customerInfoList = await _context.Users
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, group => group.ur.RoleId, r => r.Id, (group, r) => new { group, r })
                .Where(g => g.r.Name!.ToLower() == "user")
                .Select(g => new CustomerInfoDto
                {
                    UserId = g.group.u.Id,
                    Name = g.group.u.Name,
                    ImagePath = g.group.u.ImageUrl,
                    DateCreate = g.group.u.CreateDate,
                    TotalOrder = _context.Orders.Count(o => o.UserId == g.group.u.Id)
                })
                .ToListAsync();
            int TotalPage = 0;
            var _pagingFilterService = new PagingFilterService<CustomerInfoDto>();
            customerInfoList = _pagingFilterService.FilterAndPage(
                customerInfoList,
                pagingFilterDto,
                c => c.Name.ToLower().Contains(pagingFilterDto.Filter!.ToLower()),
                c => c.TotalOrder,
                ref TotalPage
            );
            return Ok(new BasePagingData<List<CustomerInfoDto>>()
            {
                TotalPage = TotalPage,
                Success = true,
                Message = $"List customer info",
                Data = customerInfoList
            });
        }
    }
}