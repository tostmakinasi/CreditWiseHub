using CreditWiseHub.Core.Dtos.Account;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.Account
{
    public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
    {
        public UpdateAccountDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

        }
    }
}
