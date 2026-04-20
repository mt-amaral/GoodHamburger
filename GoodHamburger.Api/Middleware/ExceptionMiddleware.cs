using System.Net;
using System.Text.Json;
using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Api.Middleware;

public class ExceptionMiddleware
{
    private const string InvalidRequestMessage = "Os dados da requisicao sao invalidos.";
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = _env.IsDevelopment()
                ? ex.Message
                : "Erro interno no servidor.";

            var response = CreateErrorResponse(message);

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }

    private static Response<object?> CreateErrorResponse(string message)
    {
        return CreateErrorResponse([message]);
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
