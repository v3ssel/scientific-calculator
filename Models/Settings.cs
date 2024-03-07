using System;

namespace ScientificCalculator.Models
{
    public class Settings
    {
        public bool IsSettingsSaved { get; set; }
        public bool IsHistorySaved { get; set; }
        public bool IsLogEnabled { get; set; }
        public string FirstBackgroundColor { get; set; } = string.Empty;
        public string SecondBackgroundColor { get; set; } = string.Empty;
        public string ForegroundColor { get; set; } = string.Empty;
    }
}