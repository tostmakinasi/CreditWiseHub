using CreditWiseHub.Core.Dtos.Loan;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Helpers
{
    public class LoanHelper : IDisposable
    {
        private double _requestedAmount;
        private double _interestRate;
        private int _installmentCount;
        private double _defaultInstallment;
        private DateTime _paymentStartDate;

        private double _remainingPrincipalDebt;
        private double _totalRefundAmount;

        private bool disposed = false;

        public LoanHelper(decimal requestedAmount, decimal interestRate, int installmentCount, DateTime ApprovedDate)
        {
            _requestedAmount = (double)requestedAmount;
            _interestRate = (double)interestRate/100;
            _installmentCount = installmentCount;
            _defaultInstallment = MonthlyInstallmentCalculate();
            _remainingPrincipalDebt = _requestedAmount;

            _paymentStartDate = ApprovedDate.Date.AddMonths(1);
        }
        
        public IEnumerable<PaymentPlanDto> CalculateLoanPayment()
        {
            int counter = 1;
            _totalRefundAmount = _requestedAmount;
            double paidInstalmentAmount = 0;//of
            double paidAmount = 0;//ot
            DateTime paymentDate = _paymentStartDate;
            double installment = 0;
            //List<PaymentPlanDto> paymentPlan = new List<PaymentPlanDto>();

            do
            {
                var payment = new PaymentPlanDto();
                paidInstalmentAmount = GetPaidInterestAmount(_remainingPrincipalDebt);
                _totalRefundAmount += paidInstalmentAmount;
                paidAmount = _defaultInstallment - paidInstalmentAmount;

                if (_remainingPrincipalDebt - _defaultInstallment <= 0)
                {
                    installment = paidInstalmentAmount + paidAmount;
                    payment.InstallmentAmount = (decimal)installment;
                    _remainingPrincipalDebt = 0;
                }
                else
                {
                    _remainingPrincipalDebt = _remainingPrincipalDebt - paidAmount;
                    payment.InstallmentAmount = (decimal)_defaultInstallment;

                }
                payment.RemainingPrincipalDebt = (decimal)_remainingPrincipalDebt;
                payment.PrincipalInInstallments = (decimal)paidAmount;
                payment.InterestInInstallments = (decimal)paidInstalmentAmount;
                payment.InstallmentNumber = counter;
                payment.LastPaymentDate = DateTime.SpecifyKind(paymentDate, DateTimeKind.Utc);

                //paymentPlan.Add(payment);
                yield return payment;
                counter++;
                paymentDate = paymentDate.AddMonths(1);
            } while (_remainingPrincipalDebt > 0);

            //return paymentPlan;
        }

        public double MonthlyInstallmentCalculate()
        {
            var installment = _requestedAmount * (_interestRate * Math.Pow(1 + _interestRate, _installmentCount)) / (Math.Pow(1 + _interestRate, _installmentCount) - 1);
            return Math.Round(installment, 2);
        }

        private double GetPaidInterestAmount(double amount)
        {
            var paidInterest = amount * _interestRate;
            return Math.Round(paidInterest, 2);
        }

        public double GetTotalRefoundAmount()
        {
            return _totalRefundAmount;
        }

        public void Dispose()
        {
            disposed = true;
            GC.SuppressFinalize(this);
        }

    }
}
