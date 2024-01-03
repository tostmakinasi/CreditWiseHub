using CreditWiseHub.Core.Dtos.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Account
{
    public class AffectedExternalAccountDtoValidator : AbstractValidator<AffectedExternalAccountDto>
    {
        public AffectedExternalAccountDtoValidator()
        {
            RuleFor(dto => dto.AccountNumber)
                .NotEmpty().WithMessage("Account number cannot be empty.");

            RuleFor(dto => dto.BankName)
                .NotEmpty().WithMessage("Bank name is required.");

            RuleFor(dto => dto.OwnerFullName)
                .NotEmpty().WithMessage("Owner's full name is required.");
        }
    }
}
