using System;
using System.Linq;
using System.Threading.Tasks;
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

        public delegate void RotationPeriodChanged(RotationPeriod new_value);
        public event RotationPeriodChanged? RotationPeriodChangedEvent;

        public delegate void ForegroundBrushChanged(IBrush brush);
        public event ForegroundBrushChanged? ForegroundBrushChangedEvent;

        public delegate void FirstBackgroundBrushChanged(IBrush brush);
        public event FirstBackgroundBrushChanged? FirstBackgroundBrushChangedEvent;
        
        public delegate void SecondBackgroundBrushChanged(IBrush brush);
        public event SecondBackgroundBrushChanged? SecondBackgroundBrushChangedEvent;

        #endregion
        
        #region SettingsProperties

        public static Array PossibleRotationPeriods => Enum.GetValues(typeof(RotationPeriod));

        private RotationPeriod _logsRotationPeriod = RotationPeriod.Hour;
        public RotationPeriod LogsRotationPeriod
        {
            get => _logsRotationPeriod;
            set => this.RaiseAndSetIfChanged(ref _logsRotationPeriod, value);
        }

        private bool _isSaveSettingsChecked;
        public bool IsSaveSettingsChecked
        {
            get => _isSaveSettingsChecked;
            set => this.RaiseAndSetIfChanged(ref _isSaveSettingsChecked, value);
        }
        
        private bool _isSaveHistoryChecked;
        public bool IsSaveHistoryChecked
        {
            get => _isSaveHistoryChecked;
            set => this.RaiseAndSetIfChanged(ref _isSaveHistoryChecked, value);
        }
        
        private bool _isLogEnableChecked;
        public bool IsLogEnableChecked
        {
            get => _isLogEnableChecked;
            set => this.RaiseAndSetIfChanged(ref _isLogEnableChecked, value);
        }

        private Color _foregroundColor = Colors.Black;
        public Color ForegroundColor
        {
            get => _foregroundColor;
            set => this.RaiseAndSetIfChanged(ref _foregroundColor, value);
        }

        private Color _firstBackgroundColor = Colors.White;
        public Color FirstBackgroundColor
        {
            get => _firstBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _firstBackgroundColor, value);
        }

        private Color _secondBackgroundColor = Colors.Silver;
        public Color SecondBackgroundColor
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
            
            this.WhenAnyValue(x => x.IsSaveSettingsChecked)
                .Subscribe(SaveSettingsOptionClicked);

            this.WhenAnyValue(x => x.IsSaveHistoryChecked)
                .Subscribe(SaveHistoryOptionClicked);
                
            this.WhenAnyValue(x => x.IsLogEnableChecked)
                .Subscribe(LogEnableOptionClicked);

            this.WhenAnyValue(x => x.LogsRotationPeriod)
                .Subscribe(LogsRotationPeriodChanged);
                
            this.WhenAnyValue(x => x.ForegroundColor)
                .Subscribe(ForegroundColorChangedAction);

            this.WhenAnyValue(x => x.FirstBackgroundColor)
                .Subscribe(FirstBackgroundColorChangedAction);

            this.WhenAnyValue(x => x.SecondBackgroundColor)
                .Subscribe(SecondBackgroundColorChangedAction);
        }

        public void SetupFromDatabase()
        {
            CurrentSettings = DbContext.Settings.FirstOrDefault();

            if (CurrentSettings is not null)
            {
                IsSaveSettingsChecked = CurrentSettings.IsSettingsSaved;
                IsSaveHistoryChecked = CurrentSettings.IsHistorySaved;
                IsLogEnableChecked = CurrentSettings.IsLogEnabled;
                LogsRotationPeriod = CurrentSettings.RotationPeriod;

                Color tmp_color;
                if (Color.TryParse(CurrentSettings.ForegroundColor, out tmp_color))
                    ForegroundColor = tmp_color;
                    
                if (Color.TryParse(CurrentSettings.FirstBackgroundColor, out tmp_color))
                    FirstBackgroundColor = tmp_color;
                    
                if (Color.TryParse(CurrentSettings.SecondBackgroundColor, out tmp_color))
                    SecondBackgroundColor = tmp_color;
            }
            else
            {
                CurrentSettings = new Models.Settings
                {
                    IsHistorySaved = IsSaveHistoryChecked,
                    IsSettingsSaved = IsSaveSettingsChecked,
                    IsLogEnabled = IsLogEnableChecked,
                    RotationPeriod = LogsRotationPeriod,

                    FirstBackgroundColor = FirstBackgroundColor.ToString(),
                    SecondBackgroundColor = SecondBackgroundColor.ToString(),
                    ForegroundColor = ForegroundColor.ToString()
                };

                DbContext.Settings.Add(CurrentSettings);
                DbContext.SaveChanges();
            }
        }

        public void SaveSettingsOptionClicked(bool enabled)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null)
                {
                    CurrentSettings.IsSettingsSaved = enabled;
                    await DbContext.SaveChangesAsync();
                }
            });

            SaveSettingsEnableChangedEvent?.Invoke(enabled);
        }

        public void SaveHistoryOptionClicked(bool enabled)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null && CurrentSettings.IsSettingsSaved)
                {
                    CurrentSettings.IsHistorySaved = enabled;
                    await DbContext.SaveChangesAsync();
                }
            });

            SaveHistoryEnableChangedEvent?.Invoke(enabled);
        }

        public void LogEnableOptionClicked(bool enabled)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null && CurrentSettings.IsSettingsSaved)
                {
                    CurrentSettings.IsLogEnabled = enabled;
                    await DbContext.SaveChangesAsync();
                }
            });

            LogEnableChangedEvent?.Invoke(enabled);
        }

        public void LogsRotationPeriodChanged(RotationPeriod period)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null && CurrentSettings.IsSettingsSaved)
                {
                    CurrentSettings.RotationPeriod = period;
                    await DbContext.SaveChangesAsync();
                }
            });

            RotationPeriodChangedEvent?.Invoke(period);
        }

        private void ForegroundColorChangedAction(Color color)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null && CurrentSettings.IsSettingsSaved)
                {
                    CurrentSettings.ForegroundColor = color.ToString();
                    await DbContext.SaveChangesAsync();
                }
            });

            ForegroundBrush = new SolidColorBrush(color);
            ForegroundBrushChangedEvent?.Invoke(ForegroundBrush);
        }

        private void FirstBackgroundColorChangedAction(Color color)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null && CurrentSettings.IsSettingsSaved)
                {
                    CurrentSettings.FirstBackgroundColor = color.ToString();
                    await DbContext.SaveChangesAsync();
                }
            });

            FirstBackgroundBrush = new SolidColorBrush(color);
            FirstBackgroundBrushChangedEvent?.Invoke(FirstBackgroundBrush);
        }
        
        private void SecondBackgroundColorChangedAction(Color color)
        {
            Task.Run(async () =>
            {
                if (CurrentSettings is not null && CurrentSettings.IsSettingsSaved)
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