using backend.dtos;
using backend.models;
using backend.Services.Interfaces;
using backend.UOW;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AdjustedStockDataDto> AdjustStockQuantity(AdjustStockServiceFuncDto dto)
        {
            var product = await _unitOfWork.Products.GetActiveByIdAsync(dto.Id);
            if (product is null)
            {
                return null;
            }

            if (product.StockQuantity + dto.Quantity < 0)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, requested adjustment: {dto.Quantity}.");
            }

            product.StockQuantity += dto.Quantity;
            await _unitOfWork.SaveChangesAsync();
            return new AdjustedStockDataDto
            {
                Name = product.Name,
                StockQuantity = product.StockQuantity,
                CostPrice = product.CostPrice,
                UnitPrice = product.UnitPrice,
            };
        }

        public async Task<GetProductDataDto> CreateProductAsync(CreateProductDto dto)
        {
            var IsExsitingProduct = await _unitOfWork.Products.ExistsBySkuAsync(dto.SKU);
            if (IsExsitingProduct)
            {
                throw new InvalidOperationException("Product with this SKU already exists");
            }

            var product = new Products
            {
                Name = dto.Name,
                SKU = dto.SKU,
                CategoryId = dto.CategoryId,
                UnitPrice = dto.UnitPrice,
                CostPrice = dto.CostPrice,
                StockQuantity = dto.StockQuantity,
                ReorderLevel = dto.ReorderLevel,

            };
            try
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

            var saved = await _unitOfWork.Products.GetActiveByIdAsync(product.Id);
            return MapProduct(saved!);
        }
        public async Task<PaginationResultDto<GetProductDataDto>> GetAllProducts(int page = 1, int pageSize = 20, string? category = null, bool lowStock = false)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (products, totalCount) = await _unitOfWork.Products.GetPageAsync(category, lowStock, page, pageSize);
            var data = products.Select(MapProduct).ToList();

            return new PaginationResultDto<GetProductDataDto>
            {
                Data = data,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Products.GetActiveByIdAsync(id);
            if (product is null)
            {
                return false;
            }

            product.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<List<string>> GetProductCategories()
        {
            var categories = await _unitOfWork.Repository<Categories>().GetAllAsync();
            return categories.Select(c => c.Name).ToList();
        }
        public async Task<GetProductDataDto> GetProductDataByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetActiveByIdAsync(id);
            if (product is null)
            {
                return null;
            }
            return MapProduct(product);
        }
        public async Task<GetProductDataDto> UpdateProductData(int id, UpdateProductDataDto dto)
        {
            var product = await _unitOfWork.Products.GetActiveByIdAsync(id);
            if (product is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.SKU))
                product.SKU = dto.SKU;

            if (dto.UnitPrice.HasValue)
                product.UnitPrice = dto.UnitPrice.Value;

            if (dto.CostPrice.HasValue)
                product.CostPrice = dto.CostPrice.Value;

            if (dto.StockQuantity.HasValue)
                product.StockQuantity = dto.StockQuantity.Value;

            if (dto.ReorderLevel.HasValue)
                product.ReorderLevel = dto.ReorderLevel.Value;

            await _unitOfWork.SaveChangesAsync();
            return MapProduct(product);
        }

        private static GetProductDataDto MapProduct(Products p) => new GetProductDataDto
        {
            Name = p.Name,
            SKU = p.SKU,
            CategoryId = p.CategoryId,
            UnitPrice = p.UnitPrice,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            ReorderLevel = p.ReorderLevel,
            Category = p.Category
        };
    }
}