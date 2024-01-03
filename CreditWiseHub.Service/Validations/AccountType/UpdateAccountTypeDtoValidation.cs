using CreditWiseHub.Core.Dtos.AccountType;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.AccountType
{
    public class UpdateAccountTypeDtoValidation : AbstractValidator<UpdateAccountTypeDto>
    {
        public UpdateAccountTypeDtoValidation()
        {

            RuleFor(account => account.Name)
            .MaximumLength(50).WithMessage("{PropertyName}  cannot be longer than 50 characters.");

            RuleFor(account => account.MinimumOpeningBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum opening balance must be greater than or equal 0.");

            RuleFor(account => account.Description)
                .MaximumLength(255).WithMessage("Description cannot be longer than 255 characters.");
        }
    }
}
