using CreditWiseHub.Core.Dtos.User;
using FluentValidation;

namespace CreditWiseHub.Service.Validations.User
{
    public class LoginDtoValidation : AbstractValidator<LoginDto>
    {
        public LoginDtoValidation()
        {
            RuleFor(x => x.TCKN).NotEmpty().NotNull().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("{PropertyName} is required!");
        }
    }
}
