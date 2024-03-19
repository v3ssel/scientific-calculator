using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ScientificCalculator.Services.Converters;

public class RateTypeToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int rateType && targetType.IsAssignableTo(typeof(bool)))
        {   
            bool result = true;
            if (parameter is string param && param == "not")
            {
                result = !result;
            }

            if (rateType == 0)
            {
                return result;
            }

            return !result;
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
