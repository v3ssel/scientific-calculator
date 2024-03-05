using System;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;

namespace ScientificCalculator.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private int _logsRotationPeriod;
        public int LogsRotationPeriod
        {
            get => _logsRotationPeriod;
            set => this.RaiseAndSetIfChanged(ref _logsRotationPeriod, value);
        }

        private IBrush _foregroundBrush = Brushes.Black;
        public IBrush ForegroundBrush
        {
            get => _foregroundBrush;
            set => this.RaiseAndSetIfChanged(ref _foregroundBrush, value);
        }
        
        private Avalonia.Media.Color _foregroundColor = Colors.Black;
        public Avalonia.Media.Color ForegroundColor
        {
            get => _foregroundColor;
            set => this.RaiseAndSetIfChanged(ref _foregroundColor, value);
        }

        public SettingsViewModel()
        {
            // var cp = new ColorPicker();
            // Color c = cp.Color;
            // var a = new SolidColorBrush();
            
            _logsRotationPeriod = 0;
            
            this.WhenAnyValue(x => x.ForegroundColor).Subscribe(x => ForegroundBrush = new SolidColorBrush(x));
        }

        public void SaveHistoryOptionClicked()
        {

        }
    }
}