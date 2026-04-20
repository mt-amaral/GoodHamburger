using GoodHamburger.Api.Context;
using GoodHamburger.Api.Entities;
using GoodHamburger.Api.Services;
using GoodHamburger.Shared.Dto.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace GoodHamburger.Test.Api.Services;

public class MenuItemServiceTests
{
    [Fact]
    public async Task GetActiveAsync_WithAbsoluteMenuImageUrl_ReturnsRelativePath()
    {
        var options = new DbContextOptionsBuilder<GoodHamburgerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        await using var context = new GoodHamburgerDbContext(options);
        context.MenuItems.Add(new MenuItem(
            id: 1,
            name: "x-burger",
            category: MenuItemCategory.Sandwich,
            accompanimentType: null,
            price: 5.00m,
            imageUrl: "https://localhost:7002/images/menu/x-burger.webp"));

        await context.SaveChangesAsync();

        var service = new MenuItemService(context, NullLogger<MenuItemService>.Instance);

        var (response, statusCode) = await service.GetActiveAsync(null, CancellationToken.None);

        Assert.Equal(200, statusCode);
        var item = Assert.Single(Assert.IsType<List<GoodHamburger.Shared.Dto.MenuItem.MenuItemResponseDto>>(response.Data));
        Assert.Equal("/images/menu/x-burger.webp", item.ImageUrl);
    }
}
