using System.Text.Json.Serialization;
using Orders.API.Common;

namespace Orders.API.Features.CreateOrder;

public record CreateOrderCommand(
        [property: JsonConverter(typeof(SafeStringConverter))] string CustomerId, IEnumerable<CreateOrderItem> Items); 

