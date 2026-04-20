using System.Net.Http.Json;
using GoodHamburger.Configurations;
using GoodHamburger.Services.Abstractions;
using GoodHamburger.Shared.Dto.Order;
using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Services;

public sealed class OrderServiceClient(
    IHttpClientFactory httpClientFactory,
    IApiRequestDispatcher apiRequestDispatcher) : IOrderServiceClient
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(ConfigApp.HttpClientName);
    private readonly IApiRequestDispatcher _apiRequestDispatcher = apiRequestDispatcher;

    public Task<Response<List<OrderResponseDto>?>> ListAsync(CancellationToken ct)
    {
        return _apiRequestDispatcher.SendAsync<List<OrderResponseDto>?>(
            requestCt => _client.GetAsync("order/list", requestCt),
            ct);
    }

    public Task<Response<OrderResponseDto?>> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        return _apiRequestDispatcher.SendAsync<OrderResponseDto?>(
            requestCt => _client.GetAsync($"order/{orderId}", requestCt),
            ct);
    }

    public Task<Response<OrderResponseDto?>> CreateAsync(CreateOrderRequestDto request, CancellationToken ct)
    {
        return _apiRequestDispatcher.SendAsync<OrderResponseDto?>(
            requestCt => _client.PostAsJsonAsync("order", request, requestCt),
            ct);
    }

    public Task<Response<OrderResponseDto?>> UpdateAsync(Guid orderId, UpdateOrderRequestDto request, CancellationToken ct)
    {
        return _apiRequestDispatcher.SendAsync<OrderResponseDto?>(
            requestCt => _client.PutAsJsonAsync($"order/{orderId}", request, requestCt),
            ct);
    }

    public Task<Response<object?>> DeleteAsync(Guid orderId, CancellationToken ct)
    {
        return _apiRequestDispatcher.SendAsync<object?>(
            requestCt => _client.DeleteAsync($"order/{orderId}", requestCt),
            ct);
    }
}
