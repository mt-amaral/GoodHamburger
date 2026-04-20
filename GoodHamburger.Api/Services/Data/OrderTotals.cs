namespace GoodHamburger.Api.Services.Data;

public sealed record OrderTotals(
    decimal Subtotal,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal Total);
