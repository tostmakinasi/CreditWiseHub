using CreditWiseHub.Core.Dtos.User;
using FluentValidation;
using UserUserNameValidation;

namespace CreditWiseHub.Service.Validations.User
{
    public class CreateUserDtoValidation : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidation()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("{PropertyName} is required!");

            RuleFor(x => x.Surname).NotEmpty().NotNull().WithMessage("{PropertyName} is required!");

            RuleFor(x => x.TCKN).NotEmpty().NotNull().WithMessage("{PropertyName} is required!").MaximumLength(11).WithMessage("{PropertyName} must be a maximum of 11 characters long.").MinimumLength(11).WithMessage("{PropertyName} must be a minimum of 11 characters long.");

            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("{PropertyName} is required!").EmailAddress().WithMessage("Email is not a valid email address.");

            RuleFor(p => p.DateOfBirth)
      .Must(AgeLimitValidation).WithMessage("Invalid {PropertyName}").Must(Age18Validation).WithMessage("User age must be at least 18");

            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("{PropertyName} is required!");
        }

        protected static bool Age18Validation(DateTime date)
        {
            int currentYear = DateTime.UtcNow.Year;
            int yearOfBirth = date.Year;
            if (yearOfBirth <= currentYear && yearOfBirth > (currentYear - 120) && (currentYear - yearOfBirth) > 18)
            {
                return true;
            }

            return false;
        }
        private static bool AgeLimitValidation(DateTime date)
        {
            int currentYear = DateTime.UtcNow.Year;
            int yearOfBirth = date.Year;
            if (currentYear - yearOfBirth > 18)
            {
                return true;
            }

            return false;
        }
    }
}
