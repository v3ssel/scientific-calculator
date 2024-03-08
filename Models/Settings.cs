using System;
using ScientificCalculator.Services.Logging;

namespace ScientificCalculator.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public bool IsSettingsSaved { get; set; }
        public bool IsHistorySaved { get; set; }
        public bool IsLogEnabled { get; set; }
        public RotationPeriod RotationPeriod { get; set; } = RotationPeriod.Hour; 
        public string FirstBackgroundColor { get; set; } = string.Empty;
        public string SecondBackgroundColor { get; set; } = string.Empty;
        public string ForegroundColor { get; set; } = string.Empty;
    }
}