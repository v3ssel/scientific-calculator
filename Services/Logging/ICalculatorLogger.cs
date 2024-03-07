using System.Threading.Tasks;
using ScientificCalculator.Models;

namespace ScientificCalculator.Services.Logging;

public interface ICalculatorLogger
{
    Task LogAsync(LogLevel level, HistoryRecord record);
    void Log(LogLevel level, HistoryRecord record);
    bool Enabled { get; set; }
}
