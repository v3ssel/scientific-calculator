namespace ScientificCalculator.Services.Logging;

public interface ICalculatorLoggerWithRotation
{
    RotationPeriod RotationPeriod { get; set; }
}
