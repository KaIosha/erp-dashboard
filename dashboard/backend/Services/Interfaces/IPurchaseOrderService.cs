using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PaginationResultDto<GetPurchaseOrderDto>> GetAllPurchaseOrdersAsync(string? search = null, int page = 1, int pageSize = 20);
        Task<GetPurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id);

        Task<GetPurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);

        Task<GetPurchaseOrderDto?> UpdatePurchaseOrderAsync(int id,UpdatePurchaseOrderDto dto);

        Task<bool> UpdatePurchaseOrderStatusAsync(int id,UpdatePurchaseOrderStatusDto dto);
    }
}
