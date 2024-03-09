using System.Collections.Generic;

namespace ScientificCalculator.Services.Calculation;

public class DllCalculationService : ICalculationService
{
    double ICalculationService.Calculate(string expression, string? x)
    {
        throw new System.NotImplementedException();
    }

    IList<double> ICalculationService.CalculateRange(int start, int end, string expression)
    {
        throw new System.NotImplementedException();
    }
}
