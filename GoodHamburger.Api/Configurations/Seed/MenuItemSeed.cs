using GoodHamburger.Api.Configurations.Seed.Abstraction;
using GoodHamburger.Api.Context;
using GoodHamburger.Api.Entities;
using GoodHamburger.Shared.Dto.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Configurations.Seed;

public sealed class MenuItemSeed : IAppSeed
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var context = serviceProvider.GetRequiredService<GoodHamburgerDbContext>();
        var seedItems = GetMenuItems();
        var seedIds = seedItems.Select(menuItem => menuItem.Id).ToList();

        var existingItems = await context.MenuItems
            .Where(menuItem => seedIds.Contains(menuItem.Id))
            .ToDictionaryAsync(menuItem => menuItem.Id, ct);

        var hasChanges = false;

        foreach (var seedItem in seedItems)
        {
            if (existingItems.TryGetValue(seedItem.Id, out var existingItem))
            {
                existingItem.ApplySeedData(
                    seedItem.Name,
                    seedItem.Category,
                    seedItem.AccompanimentType,
                    seedItem.Price,
                    seedItem.ImageUrl,
                    seedItem.IsActive);
            }
            else
            {
                await context.MenuItems.AddAsync(seedItem, ct);
            }

            hasChanges = true;
        }

        if (!hasChanges)
            return;

        await context.SaveChangesAsync(ct);
    }

    private static List<MenuItem> GetMenuItems()
    {
        return
        [
            new MenuItem(1, "x-burger", MenuItemCategory.Sandwich, null, 5.00m, BuildImageUrl("x-burger.webp")),
            new MenuItem(2, "x-egg", MenuItemCategory.Sandwich, null, 4.50m, BuildImageUrl("x-egg.webp")),
            new MenuItem(3, "x-bacon", MenuItemCategory.Sandwich, null, 7.00m, BuildImageUrl("x-bacon.webp")),
            new MenuItem(4, "fries", MenuItemCategory.Accompaniment, AccompanimentType.Side, 2.00m, BuildImageUrl("batata-frita.webp")),
            new MenuItem(5, "soft-drink", MenuItemCategory.Accompaniment, AccompanimentType.Drink, 2.50m, BuildImageUrl("refrigerante.webp"))
        ];
    }

    private static string BuildImageUrl(string fileName) => $"/images/menu/{fileName}";
}
