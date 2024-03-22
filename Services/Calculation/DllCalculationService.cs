using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ScientificCalculator.Models;

namespace ScientificCalculator.Services.Calculation;

public partial class DllCalculationService : ICalculationService, ICreditCalculationService, IDepositCalculationService
{
    private const string CalculationLibPath = "Libs/libCalculationLib"; 
    private const string CreditLibPath = "Libs/libCreditLib"; 
    private const string DepositLibPath = "Libs/libDepositLib"; 

    #region Calculation

    [LibraryImport(CalculationLibPath, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
    internal static partial double Calculate(string expression, string x, out string error_msg);

    [LibraryImport(CalculationLibPath, StringMarshalling = StringMarshalling.Utf8)]
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

    #endregion

    #region Credit

    [LibraryImport(CreditLibPath, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
    internal static partial void CalculateAnnuity(double amount, double percent, int term, out IntPtr payments, out IntPtr overpayments, out IntPtr fullsum, out string error_msg);

    [LibraryImport(CreditLibPath, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
    internal static partial void CalculateDifferentiated(double amount, double percent, int term, out IntPtr payments, out IntPtr overpayments, out IntPtr fullsum, out string error_msg);


    public IList<CreditResult> CalculateMonthlyPaymentsAnnuity(double amount, double percent, int term)
    {
        return CalculateMonthlyPayments(amount, percent, term, true);
    }

    public IList<CreditResult> CalculateMonthlyPaymentsDifferentiated(double amount, double percent, int term)
    {
        return CalculateMonthlyPayments(amount, percent, term, false);
    }

    private IList<CreditResult> CalculateMonthlyPayments(double amount, double percent, int term, bool type)
    {
        IntPtr payments;
        IntPtr overpayments;
        IntPtr fullsum;
        string error_msg;

        if (type)
        {
            CalculateAnnuity(amount, percent, term, out payments, out overpayments, out fullsum, out error_msg);
        }
        else
        {
            CalculateDifferentiated(amount, percent, term, out payments, out overpayments, out fullsum, out error_msg);
        }

        if (!string.IsNullOrEmpty(error_msg))
        {
            throw new ArgumentException(error_msg);
        }

        var payments_arr = PtrToArray(payments, term);
        var overpayments_arr = PtrToArray(overpayments, term);
        var fullsum_arr = PtrToArray(fullsum, term);

        return ArraysToCreditResult(payments_arr, overpayments_arr, fullsum_arr, term);
    }

    private static IList<CreditResult> ArraysToCreditResult(double[] payments, double[] overpayments, double[] fullsum, int term)
    {
        var result = new List<CreditResult>();
        for (int i = 0; i < term; i++)
        {
            result.Add(new CreditResult()
                {
                    Month = i + 1,
                    Payment = payments[i],
                    Overpay = overpayments[i],
                    Fullsum = fullsum[i]
                });
        }

        return result;
    }

    private static double[] PtrToArray(IntPtr ptr, int size)
    {
        var arr = new double[size];
        Marshal.Copy(ptr, arr, 0, size);        
        Marshal.FreeCoTaskMem(ptr);

        return arr;
    }

    #endregion

    #region Deposit

    

    #endregion
}
