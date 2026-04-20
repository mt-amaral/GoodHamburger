using Bunit;
using GoodHamburger.Components;
using GoodHamburger.Shared.Dto.MenuItem;
using Microsoft.AspNetCore.Components;

namespace GoodHamburger.Test.Front.Pages;

public class OrdersSandwichListTests : TestContext
{
    [Fact]
    public void Render_WithRelativeImageUrl_UsesRelativeImagePath()
    {
        var item = new MenuItemResponseDto(
            Id: 1,
            Name: "x-burger",
            Category: "Sandwich",
            Price: 5.00m,
            IsActive: true,
            ImageUrl: "/images/menu/x-burger.webp");

        var component = RenderComponent<OrdersSandwichList>(parameters => parameters
            .Add(parameter => parameter.Items, [item])
            .Add(parameter => parameter.SelectedItemId, null)
            .Add(parameter => parameter.SelectedItemIdChanged, EventCallback.Factory.Create<int?>(this, _ => { })));

        var image = component.Find("img");

        Assert.Equal("/images/menu/x-burger.webp", image.GetAttribute("src"));
    }

    [Fact]
    public void Render_WithoutImageUrl_UsesDefaultImagePath()
    {
        var item = new MenuItemResponseDto(
            Id: 1,
            Name: "x-burger",
            Category: "Sandwich",
            Price: 5.00m,
            IsActive: true,
            ImageUrl: null);

        var component = RenderComponent<OrdersSandwichList>(parameters => parameters
            .Add(parameter => parameter.Items, [item])
            .Add(parameter => parameter.SelectedItemId, null)
            .Add(parameter => parameter.SelectedItemIdChanged, EventCallback.Factory.Create<int?>(this, _ => { })));

        var image = component.Find("img");

        Assert.Equal("/images/menu/default.webp", image.GetAttribute("src"));
    }
}
