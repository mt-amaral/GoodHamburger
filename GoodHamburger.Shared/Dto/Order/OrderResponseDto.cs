namespace GoodHamburger.Shared.Dto.Order;

public record OrderResponseDto(
    Guid Id,
    decimal Subtotal,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal Total,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<OrderItemResponseDto> Items);
