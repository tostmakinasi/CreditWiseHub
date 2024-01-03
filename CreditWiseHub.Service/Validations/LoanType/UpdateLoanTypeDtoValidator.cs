using CreditWiseHub.Core.Dtos.LoanType;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.LoanType
{
    public class UpdateLoanTypeDtoValidator : AbstractValidator<UpdateLoanTypeDto>
    {
        public UpdateLoanTypeDtoValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().WithMessage("Loan type name cannot be empty.");

            RuleFor(dto => dto.MaxInstallmentOption).GreaterThan(0).When(dto => dto.MinInstallmentOption.HasValue)
                .WithMessage("Maximum installment option must be greater than 0 when minimum installment option is provided.");

            RuleFor(dto => dto.MinInstallmentOption).GreaterThan(0).When(dto => dto.MaxInstallmentOption.HasValue)
                .WithMessage("Minimum installment option must be greater than 0 when maximum installment option is provided.");

            RuleFor(dto => dto.MinInstallmentOption).LessThanOrEqualTo(dto => dto.MaxInstallmentOption)
                .WithMessage("Minimum installment option must be less than or equal to the maximum installment option when both are provided.");

            RuleFor(dto => dto.MinLoanAmount).GreaterThanOrEqualTo(0).When(dto => dto.MaxLoanAmount.HasValue)
                .WithMessage("Minimum loan amount cannot be less than 0 when maximum loan amount is provided.");

            RuleFor(dto => dto.MaxLoanAmount).GreaterThan(dto => dto.MinLoanAmount).When(dto => dto.MinLoanAmount.HasValue)
                .WithMessage("Maximum loan amount must be greater than the minimum loan amount when minimum loan amount is provided.");

            RuleFor(dto => dto.MinCreditScore).GreaterThanOrEqualTo(0).When(dto => dto.MaxCreditScore.HasValue)
                .WithMessage("Minimum credit score cannot be less than 0 when maximum credit score is provided.");

            RuleFor(dto => dto.MaxCreditScore).GreaterThanOrEqualTo(dto => dto.MinCreditScore).When(dto => dto.MinCreditScore.HasValue)
                .WithMessage("Maximum credit score must be greater than or equal to the minimum credit score when minimum credit score is provided.");
        }
    }
}
