using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;

namespace backend.Services.Implementations
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetPurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            var supplierExists = await _unitOfWork.Suppliers.ExistsAsync(dto.SupplierId);
            if (!supplierExists)
                throw new InvalidOperationException("Supplier not found.");

            var purchaseOrder = new PurchaseOrders
            {
                OrderDate = DateTime.UtcNow,
                ExpectedDelivery = dto.ExpectedDelivery,
                SupplierId = dto.SupplierId,
                Status = "Pending",

                Lines = dto.Lines.Select(l => new PurchaseLines
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitCost = l.UnitCost
                }).ToList()
            };
            purchaseOrder.TotalAmount = purchaseOrder.Lines.Sum(l => l.Quantity * l.UnitCost);

            await _unitOfWork.PurchaseOrders.AddAsync(purchaseOrder);
            await _unitOfWork.SaveChangesAsync();

            return new GetPurchaseOrderDto
            {
                Id = purchaseOrder.Id,
                OrderDate = purchaseOrder.OrderDate,
                ExpectedDelivery = purchaseOrder.ExpectedDelivery,
                Status = purchaseOrder.Status,
                TotalAmount = purchaseOrder.TotalAmount,
                SupplierId = purchaseOrder.SupplierId,

                Lines = purchaseOrder.Lines.Select(l => MapLine(l)).ToList()
            };
        }
        public async Task<PaginationResultDto<GetPurchaseOrderDto>> GetAllPurchaseOrdersAsync(string? search = null, int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (purchaseOrders, totalCount) = await _unitOfWork.PurchaseOrders.GetPageAsync(search, page, pageSize);
            var data = purchaseOrders.Select(p => new GetPurchaseOrderDto
            {
                Id = p.Id,
                OrderDate = p.OrderDate,
                ExpectedDelivery = p.ExpectedDelivery,
                Status = p.Status,
                TotalAmount = p.TotalAmount,
                SupplierId = p.SupplierId,
                Lines = p.Lines.Select(l => MapLine(l)).ToList()
            }).ToList();

            return new PaginationResultDto<GetPurchaseOrderDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        public async Task<GetPurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id)
        {
            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetByIdWithLinesAsync(id);
            if (purchaseOrder is null)
                return null;

            return new GetPurchaseOrderDto
            {
                Id = purchaseOrder.Id,
                OrderDate = purchaseOrder.OrderDate,
                ExpectedDelivery = purchaseOrder.ExpectedDelivery,
                Status = purchaseOrder.Status,
                TotalAmount = purchaseOrder.TotalAmount,
                SupplierId = purchaseOrder.SupplierId,
                Lines = purchaseOrder.Lines.Select(l => MapLine(l)).ToList()
            };
        }
        public async Task<GetPurchaseOrderDto?> UpdatePurchaseOrderAsync(int id, UpdatePurchaseOrderDto dto)
        {
            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetByIdWithLinesAsync(id);

            if (purchaseOrder is null)
                return null;

            if (dto.ExpectedDelivery.HasValue)
                purchaseOrder.ExpectedDelivery = dto.ExpectedDelivery.Value;

            if (dto.SupplierId.HasValue)
                purchaseOrder.SupplierId = dto.SupplierId.Value;

            if (dto.Lines is not null)
            {
                var dtoLineIds = dto.Lines.Where(l => l.Id > 0).Select(l => l.Id).ToHashSet();

                foreach (var line in purchaseOrder.Lines.Where(l => !dtoLineIds.Contains(l.Id)).ToList())
                {
                    purchaseOrder.Lines.Remove(line);
                }

                foreach (var lineDto in dto.Lines)
                {
                    var line = purchaseOrder.Lines
                        .FirstOrDefault(l => l.Id == lineDto.Id);

                    if (line is not null)
                    {
                        line.ProductId = lineDto.ProductId;
                        line.Quantity = lineDto.Quantity;
                        line.UnitCost = lineDto.UnitCost;
                    }
                    else
                    {
                        purchaseOrder.Lines.Add(new PurchaseLines
                        {
                            ProductId = lineDto.ProductId,
                            Quantity = lineDto.Quantity,
                            UnitCost = lineDto.UnitCost
                        });
                    }
                }
            }

            purchaseOrder.TotalAmount = purchaseOrder.Lines
                .Sum(l => l.Quantity * l.UnitCost);

            await _unitOfWork.SaveChangesAsync();

            return new GetPurchaseOrderDto
            {
                Id = purchaseOrder.Id,
                OrderDate = purchaseOrder.OrderDate,
                ExpectedDelivery = purchaseOrder.ExpectedDelivery,
                Status = purchaseOrder.Status,
                TotalAmount = purchaseOrder.TotalAmount,
                SupplierId = purchaseOrder.SupplierId,
                Lines = purchaseOrder.Lines.Select(l => MapLine(l)).ToList()
            };
        }
        public async Task<bool> UpdatePurchaseOrderStatusAsync(int id, UpdatePurchaseOrderStatusDto dto)
        {
            var allowedStatuses = new[] { "pending", "received", "cancelled" };
            if (!allowedStatuses.Contains(dto.Status.ToLower()))
                return false;

            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetByIdWithLinesAsync(id);
            if (purchaseOrder is null)
                return false;

            if (dto.Status.ToLower() == "received" && purchaseOrder.Status.ToLower() != "received")
            {
                var productIds = purchaseOrder.Lines.Select(l => l.ProductId).Distinct().ToList();

                var products = await _unitOfWork.Products.GetByIdsAsync(productIds);

                foreach (var product in products)
                {
                    product.StockQuantity += purchaseOrder.Lines
                        .Where(l => l.ProductId == product.Id)
                        .Sum(l => l.Quantity);
                }
            }

            purchaseOrder.Status = dto.Status;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static GetPurchaseLineDto MapLine(PurchaseLines l) => new GetPurchaseLineDto
        {
            Id = l.Id,
            ProductId = l.ProductId,
            Quantity = l.Quantity,
            UnitCost = l.UnitCost
        };
    }
}