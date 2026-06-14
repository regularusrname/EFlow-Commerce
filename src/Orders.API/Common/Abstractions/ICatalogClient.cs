namespace Orders.API.Common.Abstractions;

public interface ICatalogClient<TResponse> where TResponse : IResult<TResponse>
{
    Task<TResponse> GetProductByIdAsync(string productId, CancellationToken token);
}
