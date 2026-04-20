using GoodHamburger.Api.Entities;
using GoodHamburger.Shared.Dto.Order;

namespace GoodHamburger.Api.Services.Maps;

public static class OrderToOrderResponseDtoMap
{
    public static OrderResponseDto Map(Order order)
    {
        return new OrderResponseDto(
            order.Id,
            order.Subtotal,
            order.DiscountPercentage,
            order.DiscountAmount,
            order.Total,
            order.CreatedAt,
            order.UpdatedAt,
            order.Items
                .OrderBy(orderItem => orderItem.Category)
                .ThenBy(orderItem => orderItem.AccompanimentType)
                .Select(orderItem => new OrderItemResponseDto(
                    orderItem.Id,
                    orderItem.MenuItemId,
                    orderItem.Name,
                    orderItem.Category.ToString(),
                    orderItem.UnitPrice))
                .ToList());
    }
}
