using CreditWiseHub.Core.Dtos.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Account
{
    public class AffectedAccountDtoValidator : AbstractValidator<AffectedAccountDto>
    {
        public AffectedAccountDtoValidator()
        {
            RuleFor(dto => dto.AccountNumber)
                .NotNull().NotEmpty().WithMessage("{PropertyName} is required")
                .MinimumLength(10).WithMessage("Invalid {PropertyName}!")
                .MaximumLength(10).WithMessage("Invalid {PropertyName}!");

            RuleFor(account => account.IsExternalAccount).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(account => account.IsReceiverAccount).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");

            RuleFor(dto => dto.BankName)
                .NotEmpty().When(dto => dto.IsExternalAccount).WithMessage("Bank name is required for external accounts.");

            RuleFor(dto => dto.OwnerFullName)
                .NotEmpty().When(dto => dto.IsExternalAccount).WithMessage("Owner's full name is required for external accounts.");
        }
    }
}
