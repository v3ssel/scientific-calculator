using System;

namespace ScientificCalculator.Models
{
    public class HistoryRecord
    {
        public int Id { get; set; }
        public DateTime CalculationTime { get; set; }
        public string Expression { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}