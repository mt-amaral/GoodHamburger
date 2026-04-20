using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Shared.Dto.Order;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.V1;

public class OrderController(IOrderService orderService) : BaseController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var (data, status) = await orderService.GetAllAsync(ct);
        return StatusCode(status, data);
    }

    [HttpGet]
    [Route("{orderId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid orderId, CancellationToken ct)
    {
        var (data, status) = await orderService.GetByIdAsync(orderId, ct);
        return StatusCode(status, data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto? request, CancellationToken ct)
    {
        var validationResult = ValidateRequest(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var (data, status) = await orderService.CreateAsync(request!, ct);
        return StatusCode(status, data);
    }

    [HttpPut]
    [Route("{orderId:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid orderId,
        [FromBody] UpdateOrderRequestDto? request,
        CancellationToken ct)
    {
        var validationResult = ValidateRequest(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var (data, status) = await orderService.UpdateAsync(orderId, request!, ct);
        return StatusCode(status, data);
    }

    [HttpDelete]
    [Route("{orderId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid orderId, CancellationToken ct)
    {
        var (data, status) = await orderService.DeleteAsync(orderId, ct);
        return StatusCode(status, data);
    }
}
