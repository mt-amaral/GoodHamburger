using GoodHamburger.Api.Controllers.V1;
using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Dto.Order;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Test.Api.Controllers;

public class OrderControllerTests
{
    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsBadRequestAndDoesNotCallService()
    {
        var service = new FakeOrderService();
        var controller = new OrderController(service);

        var result = await controller.Create(new CreateOrderRequestDto(null, null, null), CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);

        var response = Assert.IsType<Response<object?>>(objectResult.Value);
        Assert.Equal("O pedido deve conter ao menos um item.", response.Message);
        Assert.Equal(["O pedido deve conter ao menos um item."], response.Errors);
        Assert.Equal(0, service.CreateCallCount);
    }

    [Fact]
    public async Task Update_WithNullRequest_ReturnsBadRequestAndDoesNotCallService()
    {
        var service = new FakeOrderService();
        var controller = new OrderController(service);

        var result = await controller.Update(Guid.NewGuid(), null, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);

        var response = Assert.IsType<Response<object?>>(objectResult.Value);
        Assert.Equal("Os dados da requisicao sao obrigatorios.", response.Message);
        Assert.Equal(["Os dados da requisicao sao obrigatorios."], response.Errors);
        Assert.Equal(0, service.UpdateCallCount);
    }

    [Fact]
    public async Task Create_WithValidRequest_CallsServiceAndReturnsServiceResponse()
    {
        var service = new FakeOrderService();
        var controller = new OrderController(service);
        var request = new CreateOrderRequestDto(1, 2, 3);

        var result = await controller.Create(request, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(1, service.CreateCallCount);
    }

    private sealed class FakeOrderService : IOrderService
    {
        public int CreateCallCount { get; private set; }

        public int UpdateCallCount { get; private set; }

        public Task<(Response<List<OrderResponseDto>?> Data, short StatusCode)> GetAllAsync(CancellationToken ct)
        {
            throw new NotSupportedException();
        }

        public Task<(Response<OrderResponseDto?> Data, short StatusCode)> GetByIdAsync(Guid id, CancellationToken ct)
        {
            throw new NotSupportedException();
        }

        public Task<(Response<OrderResponseDto?> Data, short StatusCode)> CreateAsync(
            CreateOrderRequestDto request,
            CancellationToken ct)
        {
            CreateCallCount++;

            return Task.FromResult((
                new Response<OrderResponseDto?>(
                    new OrderResponseDto(
                        Guid.NewGuid(),
                        30,
                        20,
                        6,
                        24,
                        DateTimeOffset.UtcNow,
                        DateTimeOffset.UtcNow,
                        []),
                    "Pedido criado com sucesso."),
                (short)201));
        }

        public Task<(Response<OrderResponseDto?> Data, short StatusCode)> UpdateAsync(
            Guid id,
            UpdateOrderRequestDto request,
            CancellationToken ct)
        {
            UpdateCallCount++;

            return Task.FromResult((new Response<OrderResponseDto?>(null, "Pedido atualizado com sucesso."), (short)200));
        }

        public Task<(Response<object?> Data, short StatusCode)> DeleteAsync(Guid id, CancellationToken ct)
        {
            throw new NotSupportedException();
        }
    }
}
