using CreditWiseHub.Core.Dtos.AutomaticPayment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Validations.AutomaticPayment
{
    public class CreateInvoiceAutomaticPaymentDtoValidation : AbstractValidator<CreateInvoiceAutomaticPaymentDto>
    {
        public CreateInvoiceAutomaticPaymentDtoValidation()
        {
            RuleFor(apr => apr.PaymentName)
            .NotEmpty().WithMessage("PaymentName cannot be empty.");

            RuleFor(apr => apr.PaymentAmount)
                .GreaterThan(0).WithMessage("PaymentAmount must be greater than 0.");

            RuleFor(apr => apr.PaymentDueDay)
                .LessThanOrEqualTo(30).WithMessage("PaymentDueDay must be less than or equal to 30.");

            RuleFor(apr => apr.PaymentDueCount)
                .GreaterThan(0).WithMessage("PaymentDueCount must be greater than 0.");
        }
    }
}
