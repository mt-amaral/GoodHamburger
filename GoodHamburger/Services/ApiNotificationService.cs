using GoodHamburger.Services.Abstractions;
using MudBlazor;

namespace GoodHamburger.Services;

public sealed class ApiNotificationService(ISnackbar snackbar) : IApiNotificationService
{
    public void Notify(string? message, Severity severity)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        snackbar.Add(message, severity);
    }
}
