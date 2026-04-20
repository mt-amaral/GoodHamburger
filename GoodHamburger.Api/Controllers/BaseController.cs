using GoodHamburger.Shared.Dto;
using GoodHamburger.Shared.Validation;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BaseController : ControllerBase
{
    private const string InvalidRequestMessage = "Os dados da requisicao sao invalidos.";

    protected IActionResult? ValidateRequest<TRequest>(TRequest? request)
        where TRequest : class, IValidatableRequest
    {
        if (request is null)
        {
            return CreateBadRequest(["Os dados da requisicao sao obrigatorios."]);
        }

        var validation = request.Validate();

        if (validation.IsValid)
        {
            return null;
        }

        return CreateBadRequest(validation.Errors.Select(error => error.ErrorMessage));
    }

    private static ObjectResult CreateBadRequest(IEnumerable<string?> errors)
    {
        return new ObjectResult(CreateErrorResponse(errors))
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    private static Response<object?> CreateErrorResponse(IEnumerable<string?> errors)
    {
        var normalizedErrors = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .Select(error => error!.Trim())
            .Distinct()
            .ToList();

        if (normalizedErrors.Count == 0)
        {
            normalizedErrors.Add(InvalidRequestMessage);
        }

        return new Response<object?>(null, string.Join(" | ", normalizedErrors), normalizedErrors);
    }
}
