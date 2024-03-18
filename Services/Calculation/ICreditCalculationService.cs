using System.Collections.Generic;
using ScientificCalculator.Models;

namespace ScientificCalculator.Services.Calculation;

public interface ICreditCalculationService
{
    IList<CreditResult> CalculateMonthlyPaymentsAnnuity(double amount, double percent, int term);
    IList<CreditResult> CalculateMonthlyPaymentsDifferentiated(double amount, double percent, int term);
}
