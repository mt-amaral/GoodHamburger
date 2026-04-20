using GoodHamburger.Shared.Dto.Order;

namespace GoodHamburger.Test.Shared.Dto.Order;

public class UpdateOrderRequestDtoTests
{
    [Fact]
    public void Validate_WithValidItems_IsValid()
    {
        var dto = new UpdateOrderRequestDto(1, null, 3);

        var result = dto.Validate();

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithoutItems_ReturnsExpectedError()
    {
        var dto = new UpdateOrderRequestDto(null, null, null);

        var result = dto.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "O pedido deve conter ao menos um item.");
    }
}
