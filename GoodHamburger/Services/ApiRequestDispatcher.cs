using System.Net;
using System.Text.Json;
using GoodHamburger.Services.Abstractions;
using GoodHamburger.Services.Exceptions;
using GoodHamburger.Shared.Dto;
using MudBlazor;

namespace GoodHamburger.Services;

public sealed class ApiRequestDispatcher(IApiNotificationService notificationService) : IApiRequestDispatcher
{
    private const string ApiUnavailableMessage = "Nao foi possivel se comunicar com a API.";
    private const string InvalidApiResponseMessage = "A API retornou uma resposta invalida.";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly IReadOnlyDictionary<HttpStatusCode, string> StatusCodeMessages = new Dictionary<HttpStatusCode, string>
    {
        [HttpStatusCode.BadRequest] = "Os dados da requisicao sao invalidos.",
        [HttpStatusCode.NotFound] = "Recurso nao encontrado.",
        [HttpStatusCode.MethodNotAllowed] = "Metodo nao permitido para este endpoint.",
        [HttpStatusCode.UnsupportedMediaType] = "Content-Type nao suportado. Utilize application/json."
    };

    public async Task<Response<TData>> SendAsync<TData>(
        Func<CancellationToken, Task<HttpResponseMessage>> requestFactory,
        CancellationToken ct)
    {
        try
        {
            using var response = await requestFactory(ct);
            return await BuildResponseAsync<TData>(response, ct);
        }
        catch (ApiRequestFailedException)
        {
            throw;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw CreateUnavailableException();
        }
        catch (HttpRequestException exception)
        {
            throw CreateUnavailableException(exception);
        }
    }

    private async Task<Response<TData>> BuildResponseAsync<TData>(HttpResponseMessage response, CancellationToken ct)
    {
        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var content = TryDeserialize<TData>(responseBody);

        if (content is not null)
        {
            var message = ResolveMessage(content.Message, content.Errors, response.StatusCode, response.IsSuccessStatusCode);
            notificationService.Notify(message, GetSeverity(response.StatusCode));

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiRequestFailedException(message ?? ResolveFallbackMessage(response.StatusCode));
            }

            return content;
        }

        var fallbackMessage = ResolveFallbackMessage(response.StatusCode, response.IsSuccessStatusCode);

        notificationService.Notify(fallbackMessage, GetSeverity(response.StatusCode));
        throw new ApiRequestFailedException(fallbackMessage);
    }

    private static Response<TData>? TryDeserialize<TData>(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Response<TData>>(responseBody, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? ResolveMessage(
        string? message,
        IEnumerable<string>? errors,
        HttpStatusCode statusCode,
        bool isSuccessStatusCode)
    {
        if (!string.IsNullOrWhiteSpace(message))
            return message;

        var firstError = errors?
            .FirstOrDefault(error => !string.IsNullOrWhiteSpace(error));

        if (!string.IsNullOrWhiteSpace(firstError))
            return firstError!;

        if (isSuccessStatusCode)
            return null;

        return ResolveFallbackMessage(statusCode);
    }

    private static string ResolveFallbackMessage(HttpStatusCode statusCode, bool isSuccessStatusCode = false)
    {
        if (isSuccessStatusCode)
            return InvalidApiResponseMessage;

        if (StatusCodeMessages.TryGetValue(statusCode, out var configuredMessage))
            return configuredMessage;

        return statusCode switch
        {
            >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => "Nao foi possivel processar a requisicao.",
            >= HttpStatusCode.InternalServerError => "Erro interno no servidor.",
            _ => InvalidApiResponseMessage
        };
    }

    private ApiRequestFailedException CreateUnavailableException(Exception? innerException = null)
    {
        notificationService.Notify(ApiUnavailableMessage, Severity.Error);
        return new ApiRequestFailedException(ApiUnavailableMessage, innerException);
    }

    private static Severity GetSeverity(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices => Severity.Success,
            >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => Severity.Warning,
            _ => Severity.Error
        };
    }
}
