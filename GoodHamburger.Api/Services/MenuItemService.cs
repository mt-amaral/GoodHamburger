using GoodHamburger.Api.Context;
using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Api.Services.Maps;
using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Dto.Enums;
using GoodHamburger.Shared.Dto.MenuItem;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Services;

public sealed class MenuItemService(
    GoodHamburgerDbContext context,
    ILogger<MenuItemService> logger) : IMenuItemService
{
    public async Task<(Response<List<MenuItemResponseDto>?> Data, short StatusCode)> GetActiveAsync(string? category, CancellationToken ct)
    {
        try
        {
            MenuItemCategory? parsedCategory = null;

            if (!string.IsNullOrWhiteSpace(category))
            {
                if (!Enum.TryParse<MenuItemCategory>(category, true, out var categoryValue))
                {
                    return (new Response<List<MenuItemResponseDto>?>(null, "Categoria invalida. Use Sandwich ou Accompaniment."), 400);
                }

                parsedCategory = categoryValue;
            }

            var query = context.MenuItems
                .AsNoTracking()
                .Where(menuItem => menuItem.IsActive);

            if (parsedCategory.HasValue)
            {
                query = query.Where(menuItem => menuItem.Category == parsedCategory.Value);
            }

            var items = await query
                .OrderBy(menuItem => menuItem.Category)
                .ThenBy(menuItem => menuItem.AccompanimentType)
                .ThenBy(menuItem => menuItem.Name)
                .ToListAsync(ct);

            var responseItems = items
                .Select(menuItem => MenuItemToMenuItemResponseDtoMap.Map(menuItem, NormalizeImageUrl(menuItem.ImageUrl)))
                .ToList();

            return (new Response<List<MenuItemResponseDto>?>(responseItems, message: null, errors: null), 200);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao buscar itens ativos do cardapio.");

            return (new Response<List<MenuItemResponseDto>?>(null, "Ocorreu um erro ao buscar os itens do cardapio."), 500);
        }
    }

    public async Task<(Response<MenuItemResponseDto?> Data, short StatusCode)> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            var item = await context.MenuItems
                .AsNoTracking()
                .Where(menuItem => menuItem.Id == id)
                .FirstOrDefaultAsync(ct);

            if (item is null)
            {
                return (new Response<MenuItemResponseDto?>(null, "Item nao encontrado."), 404);
            }

            var responseItem = MenuItemToMenuItemResponseDtoMap.Map(item, NormalizeImageUrl(item.ImageUrl));

            return (new Response<MenuItemResponseDto?>(responseItem, message: null, errors: null), 200);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao buscar item do cardapio por id {MenuItemId}.", id);

            return (new Response<MenuItemResponseDto?>(null, "Ocorreu um erro ao buscar o item do cardapio."), 500);
        }
    }

    private static string? NormalizeImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return null;

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var absoluteUri))
        {
            if (absoluteUri.AbsolutePath.StartsWith("/images/menu/", StringComparison.OrdinalIgnoreCase))
                return $"{absoluteUri.AbsolutePath}{absoluteUri.Query}";

            return imageUrl;
        }

        return imageUrl.StartsWith('/') ? imageUrl : $"/{imageUrl}";
    }
}
