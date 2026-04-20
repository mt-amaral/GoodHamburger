using GoodHamburger.Shared.Dto;

namespace GoodHamburger.Test.Shared.Dto;

public class PaginationRequestDtoTests
{
    [Fact]
    public void Validate_WithValidPagination_IsValid()
    {
        var dto = new PaginationRequestDto
        {
            PageNumber = 1,
            PageSize = 10
        };

        var result = dto.Validate();

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPagination_ReturnsExpectedErrors()
    {
        var dto = new PaginationRequestDto
        {
            PageNumber = 0,
            PageSize = 101
        };

        var result = dto.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "PageNumber deve ser maior que 0.");
        Assert.Contains(result.Errors, error => error.ErrorMessage == "PageSize deve estar entre 1 e 100.");
    }
}
