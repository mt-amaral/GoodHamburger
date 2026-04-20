using System.Net;
using System.Text;
using System.Text.Json;
using GoodHamburger.Services;
using GoodHamburger.Services.Abstractions;
using GoodHamburger.Services.Exceptions;
using GoodHamburger.Shared.Dto;
using MudBlazor;

namespace GoodHamburger.Test.Front.Services;

public class ApiRequestDispatcherTests
{
    [Fact]
    public async Task SendAsync_WithSuccessfulResponse_ReturnsResponseAndNotifiesSuccess()
    {
        var notificationService = new FakeApiNotificationService();
        var dispatcher = new ApiRequestDispatcher(notificationService);

        var result = await dispatcher.SendAsync<string>(
            _ => Task.FromResult(CreateJsonResponse(HttpStatusCode.Created, new Response<string>("pedido", "Pedido criado com sucesso."))),
            CancellationToken.None);

        Assert.Equal("pedido", result.Data);
        Assert.Equal("Pedido criado com sucesso.", result.Message);

        var notification = Assert.Single(notificationService.Notifications);
        Assert.Equal("Pedido criado com sucesso.", notification.Message);
        Assert.Equal(Severity.Success, notification.Severity);
    }

    [Fact]
    public async Task SendAsync_WithApiErrorResponse_NotifiesAndThrowsHandledException()
    {
        var notificationService = new FakeApiNotificationService();
        var dispatcher = new ApiRequestDispatcher(notificationService);

        var exception = await Assert.ThrowsAsync<ApiRequestFailedException>(() => dispatcher.SendAsync<object?>(
            _ => Task.FromResult(CreateJsonResponse(HttpStatusCode.NotFound, new Response<object?>(null, "Pedido nao encontrado."))),
            CancellationToken.None));

        Assert.Equal("Pedido nao encontrado.", exception.Message);

        var notification = Assert.Single(notificationService.Notifications);
        Assert.Equal("Pedido nao encontrado.", notification.Message);
        Assert.Equal(Severity.Warning, notification.Severity);
    }

    [Fact]
    public async Task SendAsync_WithEmptyNotFoundBody_UsesFallbackMessage()
    {
        var notificationService = new FakeApiNotificationService();
        var dispatcher = new ApiRequestDispatcher(notificationService);

        var exception = await Assert.ThrowsAsync<ApiRequestFailedException>(() => dispatcher.SendAsync<object?>(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            }),
            CancellationToken.None));

        Assert.Equal("Recurso nao encontrado.", exception.Message);

        var notification = Assert.Single(notificationService.Notifications);
        Assert.Equal("Recurso nao encontrado.", notification.Message);
        Assert.Equal(Severity.Warning, notification.Severity);
    }

    private static HttpResponseMessage CreateJsonResponse<TData>(HttpStatusCode statusCode, Response<TData> payload)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
                Encoding.UTF8,
                "application/json")
        };
    }

    private sealed class FakeApiNotificationService : IApiNotificationService
    {
        public List<(string? Message, Severity Severity)> Notifications { get; } = [];

        public void Notify(string? message, Severity severity)
        {
            Notifications.Add((message, severity));
        }
    }
}
