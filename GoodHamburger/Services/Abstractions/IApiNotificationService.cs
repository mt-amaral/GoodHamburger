using MudBlazor;

namespace GoodHamburger.Services.Abstractions;

public interface IApiNotificationService
{
    void Notify(string? message, Severity severity);
}
