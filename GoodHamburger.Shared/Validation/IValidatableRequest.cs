using ValidationResult = FluentValidation.Results.ValidationResult;

namespace GoodHamburger.Shared.Validation;

public interface IValidatableRequest
{
    ValidationResult Validate();
}
