using System.Collections.Generic;

namespace ScientificCalculator.Services.Calculation;

public interface ICalculationService
{
    double Calculate(string expression, string? x);
    IList<double> CalculateRange(int start, int end, string expression);
}
