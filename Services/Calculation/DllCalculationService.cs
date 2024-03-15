using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ScientificCalculator.Services.Calculation;

public partial class DllCalculationService : ICalculationService
{
    [LibraryImport("Libs/libCalculationLib.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
    internal static partial double Calculate(string expression, string x, out string error_msg);

    [LibraryImport("Libs/libCalculationLib.dll", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
    internal static partial void CalculateRange(int start, int end, string expression, out int size, out IntPtr data, out string error_msg);

    double ICalculationService.Calculate(string expression, string? x)
    {
        double result = Calculate(expression, x ?? "", out string error_msg);
        if (!string.IsNullOrEmpty(error_msg))
        {
            throw new ArgumentException(error_msg);
        }

        return result;
    }

    IList<double> ICalculationService.CalculateRange(int start, int end, string expression)
    {
        CalculateRange(start, end, expression, out int size, out IntPtr data, out string error_msg);

        if (!string.IsNullOrEmpty(error_msg))
        {
            throw new ArgumentException(error_msg);
        }

        var result = new double[size];

        Marshal.Copy(data, result, 0, size);        
        Marshal.FreeCoTaskMem(data);

        return result;
    }
}
