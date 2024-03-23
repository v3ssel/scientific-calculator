using System;
using System.Runtime.InteropServices;

namespace ScientificCalculator.Models;

[StructLayout(LayoutKind.Sequential)]
public struct DepositParams
{
    public double start_amount;
    public int term_in_days;
    public int term_begin_day;
    public int term_begin_month;
    public int term_begin_year;
    public double tax_rate;
    public int periodicity;

    [MarshalAs(UnmanagedType.U1)]
    public bool capitalization;

    public IntPtr days_of_replenishments;
    public IntPtr amount_of_replenishments;
    public int count_of_replenishments;

    public int rate_type;
    public IntPtr rates;
    public IntPtr rate_dependence_values;
    public int count_of_rates;
}
