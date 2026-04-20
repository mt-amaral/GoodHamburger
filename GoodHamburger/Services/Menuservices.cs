using GoodHamburger.Configurations;
using GoodHamburger.Services.Abstractions;
using GoodHamburger.Shared.Dto.MenuItem;
using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Services;

public class Menuservices(
    IHttpClientFactory httpClientFactory,
    IApiRequestDispatcher apiRequestDispatcher) : IMenuservices
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(ConfigApp.HttpClientName);
    private readonly IApiRequestDispatcher _apiRequestDispatcher = apiRequestDispatcher;

    public Task<Response<List<MenuItemResponseDto>?>> ListAsync(string? category, CancellationToken ct)
    {
        return _apiRequestDispatcher.SendAsync<List<MenuItemResponseDto>?>(
            requestCt => _client.GetAsync($"menu/list?category={Uri.EscapeDataString(category ?? string.Empty)}", requestCt),
            ct);
    }
}
