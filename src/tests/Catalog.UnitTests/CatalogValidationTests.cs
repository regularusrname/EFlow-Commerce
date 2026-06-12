using System.Text;
using Catalog.API.Features.CreateProduct;
using Catalog.API.Features.GetProduct;
using FluentValidation;

namespace Catalog.UnitTests;

public class CatalogValidationTests
{
    private readonly string _validDesciption = "Some Keyboard";
    private readonly string _validName = "Keyboard";
    private readonly decimal _validPrice = 69.420m;
    private readonly int _validStockQuantity = 34;

    [Fact]
    public async Task CreateProduct_SuccessValidation_ValidData()
    {
        var request = new CreateProductCommand(_validName, _validDesciption, _validPrice, _validStockQuantity);
        var validator = new CreateProductCommandValidator();

        var response = await validator.ValidateAsync(request);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async Task CreateProduct_FailureValidation_NameLengthTooShort()
    {
        var shortName = "SN";
        var request = new CreateProductCommand(shortName, _validDesciption, _validPrice, _validStockQuantity);
        var expectedErrMessage = "Invalid length of Name";
        var validator = new CreateProductCommandValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateProduct_FailureValidation_NameLengthTooLong()
    {
        var longName = CreateString(269, '@');
        var expectedErrMessage = "Invalid length of Name";
        var request = new CreateProductCommand(longName, _validDesciption, _validPrice, _validStockQuantity);
        var validator = new CreateProductCommandValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateProduct_FailureValidation_DescriptionTooLarge()
    {
        var longDescription = CreateString(300, 'D');
        var request = new CreateProductCommand(_validName, longDescription, _validPrice, _validStockQuantity);
        var expectedErrMessage = "Description is too large";
        var validator = new CreateProductCommandValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateProduct_FailureValidation_PriceLessThanZero()
    {
        var invalidPrice = -23.0m;
        var request = new CreateProductCommand(_validName, _validDesciption, invalidPrice, _validStockQuantity);
        var expectedErrMessage = "Price should be greater than 0";
        var validator = new CreateProductCommandValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateProduct_FailureValidation_StockQuantityLessThanZero()
    {
        var invalidSQ = -34;
        var request = new CreateProductCommand(_validName, _validDesciption, _validPrice, invalidSQ);
        var expectedErrMessage = "StockQuantity cannot be less than 0";
        var validator = new CreateProductCommandValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task GetProduct_SuccessValidation_StringIsParsableToGuid()
    {
        var productId = Guid.CreateVersion7().ToString();
        var request = new GetProductQuery(productId);
        var validator = new GetProductQueryValidator();

        var response = await validator.ValidateAsync(request);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async Task GetProduct_FailureValidation_EmptyGuid()
    {
        var productId = Guid.Empty.ToString();
        var request = new GetProductQuery(productId);
        var expectedErrMessage = "ProductId has invalid GUID format";
        var validator = new GetProductQueryValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task GetProduct_FailureValidation_EmptyString()
    {
        var productId = "";
        var request = new GetProductQuery(productId);
        var expectedErrMessage = "ProductId has invalid GUID format";
        var validator = new GetProductQueryValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task GetProduct_FailureValidation_WhiteSpace()
    {
        var productId = " ";
        var request = new GetProductQuery(productId);
        var expectedErrMessage = "ProductId has invalid GUID format";
        var validator = new GetProductQueryValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task GetProduct_FailureValidation_TryParseObjectFromString()
    {
        var productId = new { SomeField = "Some text", SomeValue2 = 234 }.ToString();
        var request = new GetProductQuery(productId!);
        var expectedErrMessage = "ProductId has invalid GUID format";
        var validator = new GetProductQueryValidator();

        var response = await validator.ValidateAsync(request);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal(expectedErrMessage, response.Errors.First().ErrorMessage);
    }

    private string CreateString(int length, char symbol)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
            sb.Append(symbol);
        return sb.ToString();
    }
}
