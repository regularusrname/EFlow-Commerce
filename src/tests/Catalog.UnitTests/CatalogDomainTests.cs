using Catalog.API.Domain.Products;

namespace Catalog.UnitTests;

public class CatalogDomainTests
{
    private readonly string _validDesciption = "Some Keyboard";
    private readonly string _validName = "Keyboard";
    private readonly decimal _validPrice = 69.420m;
    private readonly int _validStockQuantity = 34;

    [Fact]
    public void ProductDomain_SuccessCreation_ValidData()
    {
        var product = new Product(_validName, _validDesciption, _validPrice, _validStockQuantity);

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.NotEqual(DateTime.MinValue, product.CreatedAt);
        Assert.Equal(_validName, product.Name);
        Assert.Equal(_validDesciption, product.Description);
        Assert.Equal(_validPrice, product.Price);
        Assert.Equal(_validStockQuantity, product.StockQuantity);
    }

    [Fact]
    public void ProductDomain_SuccessCreation_DescriptionIsNull()
    {
        var product = new Product(_validName, null, _validPrice, _validStockQuantity);

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.NotEqual(DateTime.MinValue, product.CreatedAt);
        Assert.Null(product.Description);
    }

    [Fact]
    public void ProductDomain_Failure_InvalidName()
    {
        var invalidName = "";
        var expectedExceptionMessage = "Name argument should exists";

        var exception = Assert.Throws<ArgumentException>(
                () => new Product(invalidName, _validDesciption, _validPrice, _validStockQuantity)
        );

        Assert.Equal(expectedExceptionMessage, exception.Message);
    }

    [Fact]
    public void ProductDomain_Failure_InvalidPrice()
    {
        var invalidPrice = -23.23m;
        var expectedExceptionMessage = "Price argument should be greater than 0";

        var exception = Assert.Throws<ArgumentException>(
                () => new Product(_validName, _validDesciption, invalidPrice, _validStockQuantity)
        );

        Assert.Equal(expectedExceptionMessage, exception.Message);
    }

    [Fact]
    public void ProductDomain_Failure_InvalidStockQuantity()
    {
        var invalidStockQuantity = -1;
        var expectedExceptionMessage = "StockQuantity should be greater than 0";

        var exception = Assert.Throws<ArgumentException>(
                () => new Product(_validName, _validDesciption, _validPrice, invalidStockQuantity)
        );

        Assert.Equal(expectedExceptionMessage, exception.Message);
    }
}
