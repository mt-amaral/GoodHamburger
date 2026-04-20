using System.Globalization;

namespace GoodHamburger.Formatting;

public static class DisplayFormatters
{
    private static readonly NumberFormatInfo BrazilianCurrencyFormat = new()
    {
        CurrencySymbol = "R$",
        CurrencyDecimalDigits = 2,
        CurrencyDecimalSeparator = ",",
        CurrencyGroupSeparator = ".",
        CurrencyPositivePattern = 2,
        NumberDecimalDigits = 2,
        NumberDecimalSeparator = ",",
        NumberGroupSeparator = "."
    };

    public static string FormatCurrency(decimal value)
    {
        return value.ToString("C", BrazilianCurrencyFormat);
    }
}
