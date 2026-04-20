namespace GoodHamburger.Shared.Dto.MenuItem;

public record MenuItemResponseDto(
    int Id,
    string Name,
    string Category,
    decimal Price,
    bool IsActive,
    string? ImageUrl = null,
    string? AccompanimentType = null);
