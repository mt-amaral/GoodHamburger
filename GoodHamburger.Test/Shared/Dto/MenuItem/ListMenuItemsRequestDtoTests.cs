using GoodHamburger.Shared.Dto.MenuItem;

namespace GoodHamburger.Test.Shared.Dto.MenuItem;

public class ListMenuItemsRequestDtoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Sandwich")]
    [InlineData("Accompaniment")]
    [InlineData("sandwich")]
    public void Validate_WithSupportedCategory_IsValid(string? category)
    {
        var dto = new ListMenuItemsRequestDto
        {
            Category = category
        };

        var result = dto.Validate();

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithUnsupportedCategory_ReturnsExpectedError()
    {
        var dto = new ListMenuItemsRequestDto
        {
            Category = "Dessert"
        };

        var result = dto.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Categoria invalida. Use Sandwich ou Accompaniment.");
    }
}
