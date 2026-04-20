using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Shared.Dto.MenuItem;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.V1;

public class MenuController(IMenuItemService menuItemService) : BaseController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> List([FromQuery] ListMenuItemsRequestDto request, CancellationToken ct)
    {
        var validationResult = ValidateRequest(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var (data, status) = await menuItemService.GetActiveAsync(request.Category, ct);
        return StatusCode(status, data);
    }

    [HttpGet]
    [Route("{menuItemId:int}")]
    public async Task<IActionResult> GetById([FromRoute] int menuItemId, CancellationToken ct)
    {
        var (data, status) = await menuItemService.GetByIdAsync(menuItemId, ct);
        return StatusCode(status, data);
    }
}
