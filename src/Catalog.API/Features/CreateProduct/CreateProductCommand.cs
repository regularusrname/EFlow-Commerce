using System.Text.Json.Serialization;
using Catalog.API.Common;

namespace Catalog.API.Features.CreateProduct;

public record CreateProductCommand(
        string Name, 
        string? Description,
        [property:JsonConverter(typeof(SafeDecimalConverter))] decimal Price,
        [property:JsonConverter(typeof(SafeIntConverter))] int StockQuantity);
