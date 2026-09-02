using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetOrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var customerExists = await _unitOfWork.Customers.ExistsAsync(dto.CustomerId);

            if (!customerExists)
                throw new InvalidOperationException("Customer not found.");

            var productIds = dto.Lines.Select(l => l.ProductId).Distinct().ToList();

            var products = await _unitOfWork.Products.GetByIdsAsync(productIds);

            if (products.Count != productIds.Count)
                throw new InvalidOperationException("One or more products do not exist.");

            var productsById = products.ToDictionary(p => p.Id);

            var order = new Orders
            {
                OrderDate = dto.OrderDate,
                CustomerId = dto.CustomerId,
                PaymentMethod = dto.PaymentMethod,
                ShippingAddress = dto.ShippingAddress,
                Status = "Pending",
                Lines = dto.Lines.Select(l => new OrderLines
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    Discount = l.Discount
                }).ToList()
            };

            order.TotalAmount = order.Lines.Sum(l => (l.Quantity * l.UnitPrice) - l.Discount);

            foreach (var product in products)
            {
                var ordered = order.Lines
                    .Where(l => l.ProductId == product.Id)
                    .Sum(l => l.Quantity);

                if (product.StockQuantity < ordered)
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, requested: {ordered}.");

                product.StockQuantity -= ordered;
            }

            var invoice = new Invoices
            {
                InvoiceDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = "Draft",
                CustomerId = dto.CustomerId,
                Order = order,
                Lines = order.Lines.Select(l => new InvoiceLines
                {
                    Description = productsById[l.ProductId].Name,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    TaxRate = 0
                }).ToList()
            };

            invoice.TotalAmount = invoice.Lines.Sum(l => l.Quantity * l.UnitPrice);

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return new GetOrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Lines = order.Lines.Select(line => MapLine(line)).ToList()
            };
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdWithLinesAndInvoiceAsync(id);

            if (order is null)
                return false;

            if (order.Status != "Pending")
                return false;

            if (order.Invoice is not null && order.Invoice.Status == "Paid")
                return false;

            var productIds = order.Lines.Select(l => l.ProductId).Distinct().ToList();

            var products = await _unitOfWork.Products.GetByIdsAsync(productIds);

            foreach (var product in products)
            {
                product.StockQuantity += order.Lines
                    .Where(l => l.ProductId == product.Id)
                    .Sum(l => l.Quantity);
            }

            if (order.Invoice is not null)
                _unitOfWork.Invoices.Remove(order.Invoice);

            _unitOfWork.Orders.Remove(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<PaginationResultDto<GetOrderDto>> GetAllOrdersAsync(
            string? search = null,
            string? status = null,
            int? customerId = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (orders, totalCount) = await _unitOfWork.Orders.GetPageAsync(search, status, customerId, from, to, page, pageSize);
            var data = orders.Select(o => new GetOrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.OrderDate,
                ShippingAddress = o.ShippingAddress,
                TotalAmount = o.TotalAmount,
                Lines = o.Lines.Select(line => MapLine(line)).ToList()
            }).ToList();

            return new PaginationResultDto<GetOrderDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<GetOrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdWithLinesAsync(id);
            if (order is null)
                return null;

            return new GetOrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Lines = order.Lines.Select(line => MapLine(line)).ToList()
            };
        }

        public async Task<GetOrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdWithLinesAsync(id);

            if (order is null)
                return null;

            if (dto.CustomerId.HasValue)
                order.CustomerId = dto.CustomerId.Value;

            if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
                order.PaymentMethod = dto.PaymentMethod;

            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress))
                order.ShippingAddress = dto.ShippingAddress;

            var oldLines = order.Lines
                .Select(l => new { l.Id, l.ProductId, l.Quantity })
                .ToList();

            if (dto.Lines is not null)
            {
                var dtoLineIds = dto.Lines.Where(l => l.Id > 0).Select(l => l.Id).ToHashSet();

                foreach (var line in order.Lines.Where(l => !dtoLineIds.Contains(l.Id)).ToList())
                {
                    order.Lines.Remove(line);
                }

                foreach (var lineDto in dto.Lines)
                {
                    var line = order.Lines
                        .FirstOrDefault(l => l.Id == lineDto.Id);

                    if (line is not null)
                    {
                        line.ProductId = lineDto.ProductId;
                        line.Quantity = lineDto.Quantity;
                        line.UnitPrice = lineDto.UnitPrice;
                        line.Discount = lineDto.Discount;
                    }
                    else
                    {
                        order.Lines.Add(new OrderLines
                        {
                            ProductId = lineDto.ProductId,
                            Quantity = lineDto.Quantity,
                            UnitPrice = lineDto.UnitPrice,
                            Discount = lineDto.Discount
                        });
                    }
                }
            }

            order.TotalAmount = order.Lines.Sum(l => (l.Quantity * l.UnitPrice) - l.Discount);

            var affectedProductIds = oldLines.Select(l => l.ProductId)
                .Concat(order.Lines.Select(l => l.ProductId))
                .Distinct()
                .ToList();

            var products = await _unitOfWork.Products.GetByIdsAsync(affectedProductIds);

            if (products.Count != affectedProductIds.Count)
                throw new InvalidOperationException("One or more products do not exist.");

            foreach (var product in products)
            {
                var oldQuantity = oldLines
                    .Where(l => l.ProductId == product.Id)
                    .Sum(l => l.Quantity);

                var newQuantity = order.Lines
                    .Where(l => l.ProductId == product.Id)
                    .Sum(l => l.Quantity);

                var delta = oldQuantity - newQuantity;

                if (product.StockQuantity + delta < 0)
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{product.Name}'.");

                product.StockQuantity += delta;
            }

            await _unitOfWork.SaveChangesAsync();

            return new GetOrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Lines = order.Lines.Select(line => MapLine(line)).ToList()
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var allowedStatuses = new[] { "pending", "confirmed", "shipped", "delivered" };
            if (!allowedStatuses.Contains(dto.Status.ToLower()))
                return false;

            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order is null)
                return false;

            order.Status = dto.Status;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static GetOrderLineDto MapLine(OrderLines line) => new GetOrderLineDto
        {
            Id = line.Id,
            ProductId = line.ProductId,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            Discount = line.Discount
        };
    }
}