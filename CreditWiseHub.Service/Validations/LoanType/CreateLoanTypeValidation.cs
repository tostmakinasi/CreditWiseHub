using CreditWiseHub.Core.Dtos.LoanType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.LoanType
{
    public class CreateLoanTypeDtoValidator : AbstractValidator<CreateLoanTypeDto>
    {
        public CreateLoanTypeDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().WithMessage("Loan type name cannot be empty.");
            RuleFor(dto => dto.MaxInstallmentOption).GreaterThan(0).WithMessage("Maximum installment option must be greater than 0.");
            RuleFor(dto => dto.MinInstallmentOption).GreaterThan(0).WithMessage("Minimum installment option must be greater than 0.");
            RuleFor(dto => dto.MinInstallmentOption).LessThanOrEqualTo(dto => dto.MaxInstallmentOption)
                .WithMessage("Minimum installment option must be less than or equal to the maximum installment option.");

            RuleFor(dto => dto.MinLoanAmount).GreaterThanOrEqualTo(0).WithMessage("Minimum loan amount cannot be less than 0.");
            RuleFor(dto => dto.MaxLoanAmount).GreaterThan(dto => dto.MinLoanAmount)
                .WithMessage("Maximum loan amount must be greater than the minimum loan amount.");

            RuleFor(dto => dto.MinCreditScore).GreaterThanOrEqualTo(0).WithMessage("Minimum credit score cannot be less than 0.");
            RuleFor(dto => dto.MaxCreditScore).GreaterThanOrEqualTo(dto => dto.MinCreditScore)
                .WithMessage("Maximum credit score must be greater than or equal to the minimum credit score.");
        }
    }
}
