namespace Catalog.API.Features;

public record ProductResponse(
        string Id, 
        string Name, 
        string Desciption, 
        decimal Price, 
        int StockQuantity
);
