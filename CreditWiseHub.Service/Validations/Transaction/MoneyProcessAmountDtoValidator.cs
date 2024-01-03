using CreditWiseHub.Core.Dtos.Transactions;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Transaction
{
    public class MoneyProcessAmountDtoValidator : AbstractValidator<MoneyProcessAmountDto>
    {
        public MoneyProcessAmountDtoValidator()
        {
            RuleFor(dto => dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }
}
