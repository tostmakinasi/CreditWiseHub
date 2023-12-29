using CreditWiseHub.Core.Dtos.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.User
{
    public class UpdateUserDtoValidation:AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidation()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("{PropertyName} is required!").EmailAddress().WithMessage("Email is not a valid email address.");
        }
        
    }
}
