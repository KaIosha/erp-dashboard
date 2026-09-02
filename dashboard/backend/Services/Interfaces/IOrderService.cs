using backend.dtos;

namespace backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginationResultDto<GetOrderDto>> GetAllOrdersAsync(
            string? search = null,
            string? status = null,
            int? customerId = null,
            DateTime? from = null,
            DateTime? to = null,
            int page = 1,
            int pageSize = 20);
        Task<GetOrderDto?> GetOrderByIdAsync(int id);

        Task<GetOrderDto> CreateOrderAsync(CreateOrderDto dto);

        Task<GetOrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);

        Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto);

        Task<bool> DeleteOrderAsync(int id);
    }
}