using System.Runtime.InteropServices;

namespace ScientificCalculator.Models;

[StructLayout(LayoutKind.Sequential)]
public struct DepositResult
{
    public double income;
    public double tax_amount;
    public double total_amount;

    public readonly double Income => income;
    public readonly double TaxAmount => tax_amount;
    public readonly double TotalAmount => total_amount;
}
