using System.Threading.Tasks;
using ScientificCalculator.Models;

namespace ScientificCalculator.Services.Logging;

public interface ICalculatorLogger
{
    Task LogAsync(CalculationStatus level, HistoryRecord record);
    void Log(CalculationStatus level, HistoryRecord record);
    bool Enabled { get; set; }
}
