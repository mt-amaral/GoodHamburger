namespace GoodHamburger.Shared.Dto.Order;

public record OrderItemResponseDto(
    Guid Id,
    int MenuItemId,
    string Name,
    string Category,
    decimal UnitPrice);
