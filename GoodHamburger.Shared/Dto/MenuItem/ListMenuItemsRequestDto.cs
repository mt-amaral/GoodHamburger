using FluentValidation;
using GoodHamburger.Shared.Dto.Enums;
using GoodHamburger.Shared.Validation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace GoodHamburger.Shared.Dto.MenuItem;

public record ListMenuItemsRequestDto : IValidatableRequest
{
    public string? Category { get; init; }

    public ValidationResult Validate()
    {
        return new Validator().Validate(this);
    }

    private sealed class Validator : AbstractValidator<ListMenuItemsRequestDto>
    {
        public Validator()
        {
            RuleFor(request => request.Category)
                .Must(BeAValidCategory)
                .When(request => !string.IsNullOrWhiteSpace(request.Category))
                .WithMessage("Categoria invalida. Use Sandwich ou Accompaniment.");
        }

        private static bool BeAValidCategory(string? category)
        {
            return Enum.TryParse<MenuItemCategory>(category, true, out _);
        }
    }
}
