using GoodHamburger.Api.Context;
using GoodHamburger.Api.Entities;
using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Api.Services.Data;
using GoodHamburger.Api.Services.Maps;
using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Dto.Enums;
using GoodHamburger.Shared.Dto.Order;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Services;

public sealed class OrderService(
    GoodHamburgerDbContext context,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<(Response<List<OrderResponseDto>?> Data, short StatusCode)> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var orders = await context.Orders
                .AsNoTracking()
                .Include(order => order.Items)
                .OrderByDescending(order => order.CreatedAt)
                .ToListAsync(ct);

            return (new Response<List<OrderResponseDto>?>(orders.Select(OrderToOrderResponseDtoMap.Map).ToList(), message: null, errors: null), 200);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao buscar pedidos.");

            return (new Response<List<OrderResponseDto>?>(null, "Ocorreu um erro ao buscar os pedidos."), 500);
        }
    }

    public async Task<(Response<OrderResponseDto?> Data, short StatusCode)> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var order = await context.Orders
                .AsNoTracking()
                .Include(currentOrder => currentOrder.Items)
                .FirstOrDefaultAsync(currentOrder => currentOrder.Id == id, ct);

            if (order is null)
            {
                return (new Response<OrderResponseDto?>(null, "Pedido nao encontrado."), 404);
            }

            return (new Response<OrderResponseDto?>(OrderToOrderResponseDtoMap.Map(order), message: null, errors: null), 200);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao buscar pedido por id {OrderId}.", id);

            return (new Response<OrderResponseDto?>(null, "Ocorreu um erro ao buscar o pedido."), 500);
        }
    }

    public async Task<(Response<OrderResponseDto?> Data, short StatusCode)> CreateAsync(CreateOrderRequestDto request, CancellationToken ct)
    {
        try
        {
            var orderItemsResult = await BuildOrderItemsAsync(
                Guid.NewGuid(),
                request.SandwichId,
                request.SideId,
                request.DrinkId,
                ct);

            if (orderItemsResult.Error is not null)
            {
                return (new Response<OrderResponseDto?>(null, orderItemsResult.Error), 400);
            }

            var now = DateTimeOffset.UtcNow;
            var totals = CalculateTotals(orderItemsResult.Items!);

            var order = new Order(
                orderItemsResult.OrderId,
                totals.Subtotal,
                totals.DiscountPercentage,
                totals.DiscountAmount,
                totals.Total,
                now,
                now,
                orderItemsResult.Items);

            context.Orders.Add(order);
            await context.SaveChangesAsync(ct);

            return (
                new Response<OrderResponseDto?>(OrderToOrderResponseDtoMap.Map(order), message: "Pedido criado com sucesso.", errors: null),
                201);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao criar pedido.");

            return (new Response<OrderResponseDto?>(null, "Ocorreu um erro ao criar o pedido."), 500);
        }
    }

    public async Task<(Response<OrderResponseDto?> Data, short StatusCode)> UpdateAsync(
        Guid id,
        UpdateOrderRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var order = await context.Orders
                .Include(currentOrder => currentOrder.Items)
                .FirstOrDefaultAsync(currentOrder => currentOrder.Id == id, ct);

            if (order is null)
            {
                return (new Response<OrderResponseDto?>(null, "Pedido nao encontrado."), 404);
            }

            var orderItemsResult = await BuildOrderItemsAsync(order.Id, request.SandwichId, request.SideId, request.DrinkId, ct);

            if (orderItemsResult.Error is not null)
            {
                return (new Response<OrderResponseDto?>(null, orderItemsResult.Error), 400);
            }

            var newItems = orderItemsResult.Items!;
            var totals = CalculateTotals(newItems);

            context.OrderItems.RemoveRange(order.Items);
            context.OrderItems.AddRange(newItems);

            order.Update(
                totals.Subtotal,
                totals.DiscountPercentage,
                totals.DiscountAmount,
                totals.Total,
                DateTimeOffset.UtcNow,
                newItems);

            await context.SaveChangesAsync(ct);

            return (
                new Response<OrderResponseDto?>(OrderToOrderResponseDtoMap.Map(order), message: "Pedido atualizado com sucesso.", errors: null),
                200);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao atualizar pedido {OrderId}.", id);

            return (new Response<OrderResponseDto?>(null, "Ocorreu um erro ao atualizar o pedido."), 500);
        }
    }

    public async Task<(Response<object?> Data, short StatusCode)> DeleteAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(currentOrder => currentOrder.Id == id, ct);

            if (order is null)
            {
                return (new Response<object?>(null, "Pedido nao encontrado."), 404);
            }

            context.Orders.Remove(order);
            await context.SaveChangesAsync(ct);

            return (
                new Response<object?>(null, message: "Pedido removido com sucesso.", errors: null),
                200);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Erro ao remover pedido {OrderId}.", id);

            return (new Response<object?>(null, "Ocorreu um erro ao remover o pedido."), 500);
        }
    }

    private async Task<OrderItemsBuildResult> BuildOrderItemsAsync(
        Guid orderId,
        int? sandwichId,
        int? sideId,
        int? drinkId,
        CancellationToken ct)
    {
        var requestedItems = new (int? Id, MenuItemCategory Category, AccompanimentType? AccompanimentType)[]
        {
            (sandwichId, MenuItemCategory.Sandwich, null),
            (sideId, MenuItemCategory.Accompaniment, AccompanimentType.Side),
            (drinkId, MenuItemCategory.Accompaniment, AccompanimentType.Drink)
        };

        var informedIds = requestedItems
            .Where(item => item.Id.HasValue)
            .Select(item => item.Id!.Value)
            .ToArray();

        if (informedIds.Length == 0)
        {
            return new OrderItemsBuildResult(orderId, null, "O pedido deve conter ao menos um item.");
        }

        var menuItems = await context.MenuItems
            .Where(menuItem => informedIds.Contains(menuItem.Id) && menuItem.IsActive)
            .ToListAsync(ct);

        var orderItems = new List<OrderItem>();

        foreach (var requestedItem in requestedItems.Where(item => item.Id.HasValue))
        {
            var menuItem = menuItems.FirstOrDefault(currentMenuItem => currentMenuItem.Id == requestedItem.Id!.Value);

            if (menuItem is null)
            {
                return new OrderItemsBuildResult(orderId, null, "O item informado nao foi encontrado no cardapio.");
            }

            if (menuItem.Category != requestedItem.Category)
            {
                return new OrderItemsBuildResult(orderId, null, $"O item informado nao pertence a categoria {requestedItem.Category}.");
            }

            if (menuItem.AccompanimentType != requestedItem.AccompanimentType)
            {
                return new OrderItemsBuildResult(orderId, null, "O item informado nao pertence ao tipo esperado.");
            }

            orderItems.Add(new OrderItem(
                Guid.NewGuid(),
                orderId,
                menuItem.Id,
                menuItem.Name,
                menuItem.Category,
                menuItem.AccompanimentType,
                menuItem.Price));
        }

        return new OrderItemsBuildResult(orderId, orderItems, null);
    }

    // Calcula o subtotal do pedido e aplica o desconto conforme a combinacao:
    // sandwich + side + drink = 20%, sandwich + drink = 15%, sandwich + side = 10%.
    // Quando nao existe combinacao elegivel, o pedido permanece sem desconto.
    private static OrderTotals CalculateTotals(IReadOnlyCollection<OrderItem> items)
    {
        var subtotal = items.Sum(item => item.UnitPrice);
        var hasSandwich = items.Any(item => item.Category == MenuItemCategory.Sandwich);
        var hasSide = items.Any(item => item.AccompanimentType == AccompanimentType.Side);
        var hasDrink = items.Any(item => item.AccompanimentType == AccompanimentType.Drink);

        var discountPercentage = hasSandwich switch
        {
            true when hasSide && hasDrink => 20m,
            true when hasDrink => 15m,
            true when hasSide => 10m,
            _ => 0m
        };

        var discountAmount = Math.Round(subtotal * discountPercentage / 100m, 2, MidpointRounding.AwayFromZero);
        var total = Math.Round(subtotal - discountAmount, 2, MidpointRounding.AwayFromZero);

        return new OrderTotals(subtotal, discountPercentage, discountAmount, total);
    }

}
