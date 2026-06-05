using System.Text.Json.Serialization;
using Orders.API.Common;

namespace Orders.API.Features.CreateOrder;

public record CreateOrderItem(
        [property: JsonConverter(typeof(SafeStringConverter))] string ProductId, 
        [property: JsonConverter(typeof(SafeIntConverter))] int Quantity, 
        [property: JsonConverter(typeof(SafeDecimalConverter))] decimal UnitPrice);
