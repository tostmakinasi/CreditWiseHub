using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Service.Validations.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Transaction
{
    public class MoneyExternalTransferDtoValidator : AbstractValidator<MoneyExternalTransferDto>
    {
        public MoneyExternalTransferDtoValidator()
        {
            RuleFor(dto => dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(dto => dto.AccountInformation)
                .SetValidator(new AffectedExternalAccountDtoValidator());
        }
    }
}
