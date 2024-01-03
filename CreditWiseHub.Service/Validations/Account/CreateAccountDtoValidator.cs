using CreditWiseHub.Core.Dtos.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Account
{
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Account name cannot be empty.")
                .MaximumLength(255).WithMessage("Account name cannot exceed 255 characters.");

            RuleFor(dto => dto.AccountTypeId)
                .GreaterThan(0).WithMessage("Please provide a valid account type.");

            RuleFor(dto => dto.OpeningBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Opening balance must be greater than 0.");

        }
    }
}
