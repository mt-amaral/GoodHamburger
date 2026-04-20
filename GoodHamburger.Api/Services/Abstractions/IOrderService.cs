using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Dto.Order;

namespace GoodHamburger.Api.Services.Abstractions;

public interface IOrderService
{
    Task<(Response<List<OrderResponseDto>?> Data, short StatusCode)> GetAllAsync(CancellationToken ct);

    Task<(Response<OrderResponseDto?> Data, short StatusCode)> GetByIdAsync(Guid id, CancellationToken ct);

    Task<(Response<OrderResponseDto?> Data, short StatusCode)> CreateAsync(
        CreateOrderRequestDto request,
        CancellationToken ct);

    Task<(Response<OrderResponseDto?> Data, short StatusCode)> UpdateAsync(
        Guid id,
        UpdateOrderRequestDto request,
        CancellationToken ct);

    Task<(Response<object?> Data, short StatusCode)> DeleteAsync(Guid id, CancellationToken ct);
}
