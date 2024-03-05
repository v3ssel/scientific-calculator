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

        private IBrush _firstBackgroundBrush = Brushes.White;
        public IBrush FirstBackgroundBrush
        {
            get => _firstBackgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _firstBackgroundBrush, value);
        }

        private Avalonia.Media.Color _firstBackgroundColor = Colors.White;
        public Avalonia.Media.Color FirstBackgroundColor
        {
            get => _firstBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _firstBackgroundColor, value);
        }

        private IBrush _secondBackgroundBrush = Brushes.Silver;
        public IBrush SecondBackgroundBrush
        {
            get => _secondBackgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _secondBackgroundBrush, value);
        }

        private Avalonia.Media.Color _secondBackgroundColor = Colors.Silver;
        public Avalonia.Media.Color SecondBackgroundColor
        {
            get => _secondBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _secondBackgroundColor, value);
        }

        public SettingsViewModel()
        {
            _logsRotationPeriod = 1;
            
            this.WhenAnyValue(x => x.ForegroundColor).Subscribe(x => ForegroundBrush = new SolidColorBrush(x));
            this.WhenAnyValue(x => x.FirstBackgroundColor).Subscribe(x => FirstBackgroundBrush = new SolidColorBrush(x));
            this.WhenAnyValue(x => x.SecondBackgroundColor).Subscribe(x => SecondBackgroundBrush = new SolidColorBrush(x));
        }

        public void SaveHistoryOptionClicked()
        {

        }
    }
}