using GoodHamburger.Shared.Dto.Order;

namespace GoodHamburger.Test.Shared.Dto.Order;

public class CreateOrderRequestDtoTests
{
    [Fact]
    public void Validate_WithValidItems_IsValid()
    {
        var dto = new CreateOrderRequestDto(1, 2, 3);

        var result = dto.Validate();

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithoutItems_ReturnsExpectedError()
    {
        var dto = new CreateOrderRequestDto(null, null, null);

        var result = dto.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "O pedido deve conter ao menos um item.");
    }

    [Theory]
    [InlineData(0, null, null, "SandwichId deve ser maior que 0 quando informado.")]
    [InlineData(-1, null, null, "SandwichId deve ser maior que 0 quando informado.")]
    [InlineData(null, 0, null, "SideId deve ser maior que 0 quando informado.")]
    [InlineData(null, -1, null, "SideId deve ser maior que 0 quando informado.")]
    [InlineData(null, null, 0, "DrinkId deve ser maior que 0 quando informado.")]
    [InlineData(null, null, -1, "DrinkId deve ser maior que 0 quando informado.")]
    public void Validate_WithNonPositiveIds_ReturnsExpectedError(
        int? sandwichId,
        int? sideId,
        int? drinkId,
        string expectedMessage)
    {
        var dto = new CreateOrderRequestDto(sandwichId, sideId, drinkId);

        var result = dto.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedMessage);
    }
}
