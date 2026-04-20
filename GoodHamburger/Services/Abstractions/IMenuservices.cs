using GoodHamburger.Shared.Dto.MenuItem;
using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Services.Abstractions;

public interface IMenuservices
{
    Task<Response<List<MenuItemResponseDto>?>> ListAsync(string? category, CancellationToken ct);
}
