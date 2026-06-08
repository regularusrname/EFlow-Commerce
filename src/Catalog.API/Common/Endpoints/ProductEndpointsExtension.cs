using Catalog.API.Features.CreateProduct;
using Catalog.API.Features.GetProduct;
using Catalog.API.Features.GetProducts;

namespace Catalog.API.Common.Endpoints;

public static class ProuctEndpointsExtension
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        CreateProductEndpoint.Map(app);
        GetProductEndpoint.Map(app);
        GetProductsEndpoint.Map(app);
    }
}
