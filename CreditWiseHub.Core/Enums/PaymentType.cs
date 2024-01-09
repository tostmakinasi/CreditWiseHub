using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Enums
{
    /// <summary>
    /// Enum representing the type of payment for automatic payments.
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Indicates a payment for a loan.
        /// </summary>
        Loan = 1,

        /// <summary>
        /// Indicates a payment for an invoice.
        /// </summary>
        Invoice
    }
}
