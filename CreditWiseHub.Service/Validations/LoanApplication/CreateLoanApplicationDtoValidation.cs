using CreditWiseHub.Core.Dtos.LoanApplication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.LoanApplication
{
    public class CreateLoanApplicationDtoValidation : AbstractValidator<CreateLoanApplicationDto>
    {
        public CreateLoanApplicationDtoValidation()
        {
            RuleFor(dto => dto.LoanTypeId).GreaterThan(0).WithMessage("LoanTypeId must be greater than 0.");
            RuleFor(dto => dto.RequestedAmount).GreaterThan(0).WithMessage("RequestedAmount must be greater than 0.");
            RuleFor(dto => dto.InstallmentCount).GreaterThan(0).WithMessage("InstallmentCount must be greater than 0.");
        }
    }
}
