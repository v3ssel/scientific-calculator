using System;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;

namespace ScientificCalculator.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Events

        public delegate void LogEnableChanged(bool new_value);
        public event LogEnableChanged? LogEnableChangedEvent;

        public delegate void SaveHistoryEnableChanged(bool new_value);
        public event SaveHistoryEnableChanged? SaveHistoryEnableChangedEvent;

        public delegate void SaveSettingsEnableChanged(bool new_value);
        public event SaveSettingsEnableChanged? SaveSettingsEnableChangedEvent;

        public delegate void ForegroundBrushChanged(IBrush brush);
        public event ForegroundBrushChanged? ForegroundBrushChangedEvent;

        public delegate void FirstBackgroundBrushChanged(IBrush brush);
        public event FirstBackgroundBrushChanged? FirstBackgroundBrushChangedEvent;
        
        public delegate void SecondBackgroundBrushChanged(IBrush brush);
        public event SecondBackgroundBrushChanged? SecondBackgroundBrushChangedEvent;

        #endregion
        
        private int _logsRotationPeriod;
        public int LogsRotationPeriod
        {
            get => _logsRotationPeriod;
            set => this.RaiseAndSetIfChanged(ref _logsRotationPeriod, value);
        }

        private bool _isSaveSettingsChecked = true;
        public bool IsSaveSettingsChecked
        {
            get => _isSaveSettingsChecked;
            set => this.RaiseAndSetIfChanged(ref _isSaveSettingsChecked, value);
        }
        
        private bool _isSaveHistoryChecked = true;
        public bool IsSaveHistoryChecked
        {
            get => _isSaveHistoryChecked;
            set => this.RaiseAndSetIfChanged(ref _isSaveHistoryChecked, value);
        }
        
        private bool _isLogEnableChecked = true;
        public bool IsLogEnableChecked
        {
            get => _isLogEnableChecked;
            set => this.RaiseAndSetIfChanged(ref _isLogEnableChecked, value);
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
            
            this.WhenAnyValue(x => x.ForegroundColor)
                .Subscribe(x =>
                {
                    ForegroundBrush = new SolidColorBrush(x);
                    ForegroundBrushChangedEvent?.Invoke(ForegroundBrush);
                });

            this.WhenAnyValue(x => x.FirstBackgroundColor)
                .Subscribe(x =>
                {
                    FirstBackgroundBrush = new SolidColorBrush(x);
                    FirstBackgroundBrushChangedEvent?.Invoke(FirstBackgroundBrush);
                });

            this.WhenAnyValue(x => x.SecondBackgroundColor)
                .Subscribe(x =>
                {
                    SecondBackgroundBrush = new SolidColorBrush(x);
                    SecondBackgroundBrushChangedEvent?.Invoke(SecondBackgroundBrush);
                });
        }

        public void SaveSettingsOptionClicked()
        {
            SaveSettingsEnableChangedEvent?.Invoke(IsSaveSettingsChecked);
        }

        public void SaveHistoryOptionClicked()
        {
            SaveHistoryEnableChangedEvent?.Invoke(IsSaveHistoryChecked);
        }

        public void LogEnableOptionClicked()
        {
            LogEnableChangedEvent?.Invoke(IsLogEnableChecked);
        }
    }
}