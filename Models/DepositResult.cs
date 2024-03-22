using System.Runtime.InteropServices;

namespace ScientificCalculator.Models;

[StructLayout(LayoutKind.Sequential)]
public struct DepositResult
{
    public double income;
    public double tax_amount;
    public double total_amount;
}
