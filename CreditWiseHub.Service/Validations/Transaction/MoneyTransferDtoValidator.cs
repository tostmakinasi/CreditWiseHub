using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Service.Validations.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Transaction
{
    public class MoneyTransferDtoValidator : AbstractValidator<MoneyTransferDto>
    {
        public MoneyTransferDtoValidator()
        {
            RuleFor(dto => dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(dto => dto.AccountInformation)
                .SetValidator(new AffectedInBankAccountDtoValidator());
        }
    }
}
