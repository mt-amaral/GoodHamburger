using GoodHamburger.Api.Entities;
using GoodHamburger.Shared.Dto.Enums;

namespace GoodHamburger.Test.Api.Entities;

public sealed class EntityValidationTests
{
    [Fact]
    public void MenuItem_WhenCategoryIsInvalid_Throws()
    {
        var action = () => new MenuItem(
            id: 1,
            name: "x-burger",
            category: (MenuItemCategory)99,
            accompanimentType: null,
            price: 5m,
            imageUrl: "/images/menu/x-burger.webp");

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void MenuItem_WhenSandwichHasAccompanimentType_Throws()
    {
        var action = () => new MenuItem(
            id: 1,
            name: "x-burger",
            category: MenuItemCategory.Sandwich,
            accompanimentType: AccompanimentType.Side,
            price: 5m,
            imageUrl: "/images/menu/x-burger.webp");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void MenuItem_WhenAccompanimentHasNoType_Throws()
    {
        var action = () => new MenuItem(
            id: 4,
            name: "fries",
            category: MenuItemCategory.Accompaniment,
            accompanimentType: null,
            price: 2m,
            imageUrl: "/images/menu/fries.webp");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void OrderItem_WhenAccompanimentTypeIsInvalid_Throws()
    {
        var action = () => new OrderItem(
            id: Guid.NewGuid(),
            orderId: Guid.NewGuid(),
            menuItemId: 4,
            name: "fries",
            category: MenuItemCategory.Accompaniment,
            accompanimentType: (AccompanimentType)99,
            unitPrice: 2m);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void OrderItem_WhenUnitPriceIsNegative_Throws()
    {
        var action = () => new OrderItem(
            id: Guid.NewGuid(),
            orderId: Guid.NewGuid(),
            menuItemId: 4,
            name: "fries",
            category: MenuItemCategory.Accompaniment,
            accompanimentType: AccompanimentType.Side,
            unitPrice: -1m);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}
