namespace Catalog.API.Common.Abstractions;

public interface IRequestHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken token);
}
