using System;

namespace ScientificCalculator.Models;

public class DepositReplenishment
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public double Amount { get; set; }
}
