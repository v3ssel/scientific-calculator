using System.Collections.Generic;
using ScientificCalculator.Models;

namespace ScientificCalculator.Services.Calculation;

public interface IDepositCalculationService
{
    DepositResult CalculateDepositIncome(DepositParams p);
}
