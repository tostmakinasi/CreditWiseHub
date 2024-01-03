using CreditWiseHub.Core.Dtos.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Account
{
    public class AffectedInBankAccountDtoValidator : AbstractValidator<AffectedInBankAccountDto>
    {
        public AffectedInBankAccountDtoValidator()
        {
            RuleFor(dto => dto.AccountNumber)
                .NotEmpty().WithMessage("Account number cannot be empty.");

            RuleFor(dto => dto.AccountHolderFullName)
                .NotEmpty().WithMessage("Account holder's full name cannot be empty.");
        }
    }
}
