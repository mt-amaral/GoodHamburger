using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Services.Abstractions;

public interface IApiRequestDispatcher
{
    Task<Response<TData>> SendAsync<TData>(
        Func<CancellationToken, Task<HttpResponseMessage>> requestFactory,
        CancellationToken ct);
}
