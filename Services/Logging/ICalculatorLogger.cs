using System.Threading.Tasks;

namespace ScientificCalculator.Services.Logging;

public interface ICalculatorLogger
{
    Task LogAsync(LogLevel level, string input, string? output);
    void Log(LogLevel level, string input, string? output);
    bool Enabled { get; set; }
}
