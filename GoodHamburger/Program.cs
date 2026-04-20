using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GoodHamburger;
using GoodHamburger.Configurations;
using GoodHamburger.Services;
using GoodHamburger.Services.Abstractions;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<global::GoodHamburger.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var backendUrl = builder.Configuration["Api:BaseUrl"];

if (!string.IsNullOrWhiteSpace(backendUrl))
{
    ConfigApp.BackendUrl = backendUrl;
}

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(ConfigApp.BackendUrl) });
builder.Services.AddHttpClient(ConfigApp.HttpClientName, client =>
{
    client.BaseAddress = new Uri(ConfigApp.BackendUrl);
});
builder.Services.AddScoped<IApiNotificationService, ApiNotificationService>();
builder.Services.AddScoped<IApiRequestDispatcher, ApiRequestDispatcher>();
builder.Services.AddScoped<IMenuservices, Menuservices>();
builder.Services.AddScoped<IOrderServiceClient, OrderServiceClient>();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
});

await builder.Build().RunAsync();
