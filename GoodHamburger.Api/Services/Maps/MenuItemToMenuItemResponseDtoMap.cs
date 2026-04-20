using GoodHamburger.Api.Entities;
using GoodHamburger.Shared.Dto.MenuItem;

namespace GoodHamburger.Api.Services.Maps;

public static class MenuItemToMenuItemResponseDtoMap
{
    public static MenuItemResponseDto Map(MenuItem menuItem, string? imageUrl)
    {
        return new MenuItemResponseDto(
            menuItem.Id,
            menuItem.Name,
            menuItem.Category.ToString(),
            menuItem.Price,
            menuItem.IsActive,
            imageUrl,
            menuItem.AccompanimentType?.ToString());
    }
}
