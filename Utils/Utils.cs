using System.Globalization;
using Avalonia.Data;

namespace ScientificCalculator.Utils;

public static class Utils
{
    public static void CheckDouble(string? str)
    {
        if (string.IsNullOrEmpty(str) || !double.TryParse(str, CultureInfo.InvariantCulture, out var _))
        {
            throw new DataValidationException("Value must be a number.");
        }
    }
}
