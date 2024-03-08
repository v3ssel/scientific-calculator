using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Services.Logging;
using ScientificCalculator.Services.Saving;

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
        
        #region SettingsProperties

        private RotationPeriod _logsRotationPeriod = RotationPeriod.Hour;
        public RotationPeriod LogsRotationPeriod
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

        #endregion

        private readonly ApplicationContext DbContext;
        private Models.Settings? CurrentSettings;

        public SettingsViewModel()
        {
            DbContext = new ApplicationContext();
            DbContext.Database.EnsureCreated();
            
            this.WhenAnyValue(x => x.ForegroundColor)
                .Subscribe(ForegroundColorChangedAction);

            this.WhenAnyValue(x => x.FirstBackgroundColor)
                .Subscribe(FirstBackgroundColorChangedAction);

            this.WhenAnyValue(x => x.SecondBackgroundColor)
                .Subscribe(SecondBackgroundColorChangedAction);

            SetupFromDatabase();
        }

        private void SetupFromDatabase()
        {
            CurrentSettings = DbContext.Settings.FirstOrDefault();

            if (CurrentSettings is not null)
            {
                IsSaveSettingsChecked = CurrentSettings.IsSettingsSaved;
                IsSaveHistoryChecked = CurrentSettings.IsHistorySaved;
                IsLogEnableChecked = CurrentSettings.IsLogEnabled;

                Avalonia.Media.Color tmp_color;
                if (Avalonia.Media.Color.TryParse(CurrentSettings.ForegroundColor, out tmp_color))
                    _foregroundColor = tmp_color;
                    
                if (Avalonia.Media.Color.TryParse(CurrentSettings.FirstBackgroundColor, out tmp_color))
                    _firstBackgroundColor = tmp_color;
                    
                if (Avalonia.Media.Color.TryParse(CurrentSettings.SecondBackgroundColor, out tmp_color))
                    _secondBackgroundColor = tmp_color;

                ForegroundBrush = new SolidColorBrush(ForegroundColor);
                FirstBackgroundBrush = new SolidColorBrush(FirstBackgroundColor);
                SecondBackgroundBrush = new SolidColorBrush(SecondBackgroundColor);

                ForegroundBrushChangedEvent?.Invoke(ForegroundBrush);
                FirstBackgroundBrushChangedEvent?.Invoke(FirstBackgroundBrush);
                SecondBackgroundBrushChangedEvent?.Invoke(SecondBackgroundBrush);
            }
            else
            {
                CurrentSettings = new Models.Settings
                {
                    IsHistorySaved = IsSaveHistoryChecked,
                    IsSettingsSaved = IsSaveSettingsChecked,
                    IsLogEnabled = IsLogEnableChecked,

                    FirstBackgroundColor = FirstBackgroundColor.ToString(),
                    SecondBackgroundColor = SecondBackgroundColor.ToString(),
                    ForegroundColor = ForegroundColor.ToString()
                };

                DbContext.Settings.Add(CurrentSettings);
                DbContext.SaveChanges();
            }
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

        private void ForegroundColorChangedAction(Avalonia.Media.Color color)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null)
                {
                    CurrentSettings.ForegroundColor = color.ToString();
                    await DbContext.SaveChangesAsync();
                }
            });

            ForegroundBrush = new SolidColorBrush(color);
            ForegroundBrushChangedEvent?.Invoke(ForegroundBrush);
        }

        private void FirstBackgroundColorChangedAction(Avalonia.Media.Color color)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null)
                {
                    CurrentSettings.FirstBackgroundColor = color.ToString();
                    await DbContext.SaveChangesAsync();
                }
            });

            FirstBackgroundBrush = new SolidColorBrush(color);
            FirstBackgroundBrushChangedEvent?.Invoke(FirstBackgroundBrush);
        }
        
        private void SecondBackgroundColorChangedAction(Avalonia.Media.Color color)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null)
                {
                    CurrentSettings.SecondBackgroundColor = color.ToString();
                    await DbContext.SaveChangesAsync();
                }
            });

            SecondBackgroundBrush = new SolidColorBrush(color);
            SecondBackgroundBrushChangedEvent?.Invoke(SecondBackgroundBrush);
        }
    }
}