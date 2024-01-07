using CreditWiseHub.Core.Dtos.Ticket;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.Ticket
{
    public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
    {
        public CreateTicketDtoValidator()
        {
            RuleFor(x=> x.Title).NotEmpty().NotNull().WithMessage("{PropertyName} required");
            RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("{PropertyName} required");

        }
    }
}
