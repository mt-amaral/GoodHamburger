using FluentValidation;
using GoodHamburger.Shared.Validation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace GoodHamburger.Shared.Dto.Order;

public record CreateOrderRequestDto(
    int? SandwichId,
    int? SideId,
    int? DrinkId) : IValidatableRequest
{
    public ValidationResult Validate()
    {
        return new Validator().Validate(this);
    }

    private sealed class Validator : AbstractValidator<CreateOrderRequestDto>
    {
        public Validator()
        {
            RuleFor(request => request)
                .Must(request => request.SandwichId.HasValue || request.SideId.HasValue || request.DrinkId.HasValue)
                .WithMessage("O pedido deve conter ao menos um item.");

            RuleFor(request => request.SandwichId)
                .GreaterThan(0)
                .When(request => request.SandwichId.HasValue)
                .WithMessage("SandwichId deve ser maior que 0 quando informado.");

            RuleFor(request => request.SideId)
                .GreaterThan(0)
                .When(request => request.SideId.HasValue)
                .WithMessage("SideId deve ser maior que 0 quando informado.");

            RuleFor(request => request.DrinkId)
                .GreaterThan(0)
                .When(request => request.DrinkId.HasValue)
                .WithMessage("DrinkId deve ser maior que 0 quando informado.");
        }
    }
}
