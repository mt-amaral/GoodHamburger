using GoodHamburger.Shared.Dto.Order;
using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Services.Abstractions;

public interface IOrderServiceClient
{
    Task<Response<List<OrderResponseDto>?>> ListAsync(CancellationToken ct);
    Task<Response<OrderResponseDto?>> GetByIdAsync(Guid orderId, CancellationToken ct);
    Task<Response<OrderResponseDto?>> CreateAsync(CreateOrderRequestDto request, CancellationToken ct);
    Task<Response<OrderResponseDto?>> UpdateAsync(Guid orderId, UpdateOrderRequestDto request, CancellationToken ct);
    Task<Response<object?>> DeleteAsync(Guid orderId, CancellationToken ct);
}
