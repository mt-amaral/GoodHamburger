using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Dto.MenuItem;

namespace GoodHamburger.Api.Services.Abstractions;

public interface IMenuItemService
{
    Task<(Response<List<MenuItemResponseDto>?> Data, short StatusCode)> GetActiveAsync(string? category, CancellationToken ct);

    Task<(Response<MenuItemResponseDto?> Data, short StatusCode)> GetByIdAsync(int id, CancellationToken ct);
}
