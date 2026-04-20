using MudBlazor;
using MudBlazor.Utilities;

namespace GoodHamburger.Configurations;

public static class ConfigApp
{
    public const string HttpClientName = "GoodHamburgerApi";

    public static string BackendUrl { get; set; } = "https://localhost:7002/api/v1/";

    public static MudTheme Theme { get; } = new()
    {
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Raleway", "sans-serif"]
            }
        },
        PaletteLight = new PaletteLight
        {
            Primary = new MudColor("#B71C1C"),
            PrimaryContrastText = Colors.Shades.White,
            Secondary = new MudColor("#E53935"),
            Background = new MudColor("#FFF5F5"),
            AppbarBackground = new MudColor("#8E1212"),
            AppbarText = Colors.Shades.White,
            TextPrimary = new MudColor("#2D1111"),
            DrawerText = Colors.Shades.White,
            DrawerBackground = new MudColor("#5F0F0F")
        },
        PaletteDark = new PaletteDark
        {
            Primary = new MudColor("#FF6B6B"),
            Secondary = new MudColor("#D32F2F"),
            Background = new MudColor("#1C1010"),
            AppbarBackground = new MudColor("#7F1010"),
            AppbarText = Colors.Shades.White,
            PrimaryContrastText = Colors.Shades.White
        }
    };
}
