using GoodHamburger.Api.Controllers.V1;
using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Dto.MenuItem;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Test.Api.Controllers;

public class MenuControllerTests
{
    [Fact]
    public async Task List_WithInvalidRequest_ReturnsBadRequestAndDoesNotCallService()
    {
        var service = new FakeMenuItemService();
        var controller = new MenuController(service);

        var result = await controller.List(new ListMenuItemsRequestDto { Category = "Dessert" }, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);

        var response = Assert.IsType<Response<object?>>(objectResult.Value);
        Assert.Equal("Categoria invalida. Use Sandwich ou Accompaniment.", response.Message);
        Assert.Equal(["Categoria invalida. Use Sandwich ou Accompaniment."], response.Errors);
        Assert.Equal(0, service.GetActiveCallCount);
    }

    [Fact]
    public async Task List_WithValidRequest_CallsService()
    {
        var service = new FakeMenuItemService();
        var controller = new MenuController(service);

        var result = await controller.List(new ListMenuItemsRequestDto { Category = "Sandwich" }, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(1, service.GetActiveCallCount);
    }

    private sealed class FakeMenuItemService : IMenuItemService
    {
        public int GetActiveCallCount { get; private set; }

        public Task<(Response<List<MenuItemResponseDto>?> Data, short StatusCode)> GetActiveAsync(string? category, CancellationToken ct)
        {
            GetActiveCallCount++;
            return Task.FromResult((new Response<List<MenuItemResponseDto>?>([], null, null), (short)200));
        }

        public Task<(Response<MenuItemResponseDto?> Data, short StatusCode)> GetByIdAsync(int id, CancellationToken ct)
        {
            throw new NotSupportedException();
        }
    }
}
