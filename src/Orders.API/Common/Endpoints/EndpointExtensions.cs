using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;

namespace Orders.API.Common.Endpoints;

public static class EndpointExtensions
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        CreateOrderEndpoint.Map(app);
        GetOrderEndpoint.Map(app);
    }
}
