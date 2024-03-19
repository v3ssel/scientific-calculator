using System;

namespace ScientificCalculator.Models;

public class DepositGridItem
{
    public int Id { get; set; }
    public string Parameter { get; set; } = null!;
    public double Value { get; set; }
}
