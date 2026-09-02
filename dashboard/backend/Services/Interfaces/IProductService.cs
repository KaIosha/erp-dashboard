using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginationResultDto<GetProductDataDto>> GetAllProducts(int page = 1, int pageSize = 20, string? category = null, bool lowStock = false);
        Task<GetProductDataDto> GetProductDataByIdAsync(int id);
        Task<GetProductDataDto> UpdateProductData(int id, UpdateProductDataDto dto);
        Task<bool> DeleteProduct(int id);
        Task<GetProductDataDto> CreateProductAsync(CreateProductDto dto);
        Task<List<string>> GetProductCategories();
        Task<AdjustedStockDataDto> AdjustStockQuantity(AdjustStockServiceFuncDto dto);
    }
}
