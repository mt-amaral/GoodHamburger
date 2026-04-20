using FluentValidation;
using GoodHamburger.Shared.Validation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace GoodHamburger.Shared.Dto;

public record PaginationRequestDto : IValidatableRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public ValidationResult Validate()
    {
        return new Validator().Validate(this);
    }

    private sealed class Validator : AbstractValidator<PaginationRequestDto>
    {
        public Validator()
        {
            RuleFor(request => request.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber deve ser maior que 0.");

            RuleFor(request => request.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize deve estar entre 1 e 100.");
        }
    }
}
