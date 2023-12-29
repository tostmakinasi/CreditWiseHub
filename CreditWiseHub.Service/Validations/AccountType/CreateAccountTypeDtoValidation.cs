using CreditWiseHub.Core.Dtos.AccountType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.AccountType
{
    public class CreateAccountTypeDtoValidation : AbstractValidator<CreateAccountTypeDto>
    {
        public CreateAccountTypeDtoValidation()
        {
            RuleFor(account => account.Name)
            .NotEmpty().WithMessage("{PropertyName}  cannot be empty.")
            .MaximumLength(50).WithMessage("{PropertyName}  cannot be longer than 50 characters.");

            RuleFor(account => account.MinimumOpeningBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum opening balance must be greater than or equal 0.");

            RuleFor(account => account.Description)
                .MaximumLength(255).WithMessage("Description cannot be longer than 255 characters.");
        }
    }
}
