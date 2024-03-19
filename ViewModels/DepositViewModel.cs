using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Views;
using static ScientificCalculator.Utils.Utils;

namespace ScientificCalculator.ViewModels
{
    public class DepositViewModel : ViewModelBase
    {
        #region Events

        public delegate void UpdateForegroundBrush(IBrush brush);
        public event UpdateForegroundBrush? ForegroundBrushChanged;

        public delegate void UpdateFirstBackgroundBrush(IBrush brush);
        public event UpdateFirstBackgroundBrush? FirstBackgroundBrushChanged;
        
        public delegate void UpdateSecondBackgroundBrush(IBrush brush);
        public event UpdateSecondBackgroundBrush? SecondBackgroundBrushChanged;

        #endregion
        
        public DepositGridViewModel ReplenishmentViewModel { get; }
        public DepositGridViewModel WithdrawalViewModel { get; }
        public DepositGridViewModel RatesViewModel { get; }
        public DepositMainViewModel DepositMainViewModel { get; }

        private ViewModelBase _contentViewModel;

        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        public ICommand OnAddReplenishmentCommand { get; }
        public ICommand OnAddWithdrawalCommand { get; }
        public ICommand OnAddInterestRateCommand { get; }

        public DepositViewModel()
        {
            DepositMainViewModel = new DepositMainViewModel();
            _contentViewModel = DepositMainViewModel;

            ReplenishmentViewModel = new DepositGridViewModel()
            {
                Title = "Replenishments",
                FirstColumnName = "Id",
                SecondColumnName = "Date",
                ThirdColumnName = "Amount"
            };
            
            WithdrawalViewModel = new DepositGridViewModel()
            {
                Title = "Withdrawals",
                FirstColumnName = "Id",
                SecondColumnName = "Date",
                ThirdColumnName = "Amount"
            };

            RatesViewModel = new DepositGridViewModel()
            {
                Title = "Rate Depending",
                FirstColumnName = "Id",
                SecondColumnName = "",
                ThirdColumnName = "Rate"
            };

            OnAddReplenishmentCommand = ReactiveCommand.Create<TextBox>(OnAddReplenishment);
            OnAddWithdrawalCommand = ReactiveCommand.Create<TextBox>(OnAddWithdrawal);
            OnAddInterestRateCommand = ReactiveCommand.Create<object>(OnAddInterestRate);

            SetupColors(DepositMainViewModel);
            SetupColors(ReplenishmentViewModel);
            SetupColors(WithdrawalViewModel);
            SetupColors(RatesViewModel);

            this.WhenAnyValue(x => x.DepositMainViewModel.SelectedRateType).Subscribe(x =>
            {
                if (x == 1)
                {
                    DepositMainViewModel.DependentValueLabel = "Amount with which the rate is valid";
                    RatesViewModel.SecondColumnName = "Amount";
                }
                if (x == 2)
                {
                    DepositMainViewModel.DependentValueLabel = "Number of the day from which the rate is valid";
                    RatesViewModel.SecondColumnName = "Day Number";
                }

                RatesViewModel.Items.Clear();
            });
        }

        public void OnAddReplenishment(TextBox textBox)
        {
            if (!double.TryParse(DepositMainViewModel.CurrentReplenishment, CultureInfo.InvariantCulture, out var replenishment))
            {
                DataValidationErrors.SetError(textBox, new DataValidationException("Amount must be a number."));
                return;
            }

            ReplenishmentViewModel.Items.Add(new DepositGridItem()
            {
                Id = ReplenishmentViewModel.Items.Count + 1, 
                Parameter = DepositMainViewModel.CurrentReplenishmentDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("en-US")),
                Value = replenishment
            });

            DataValidationErrors.ClearErrors(textBox);
        }

        public void OnViewReplenishments()
        {
            ContentViewModel = ReplenishmentViewModel;
        }

        public void OnAddWithdrawal(TextBox textBox)
        {
            if (!double.TryParse(DepositMainViewModel.CurrentWithdrawal, CultureInfo.InvariantCulture, out var withdrawal))
            {
                DataValidationErrors.SetError(textBox, new DataValidationException("Amount must be a number."));
                return;
            }

            WithdrawalViewModel.Items.Add(new DepositGridItem()
            {
                Id = WithdrawalViewModel.Items.Count + 1, 
                Parameter = DepositMainViewModel.CurrentWithdrawalDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("en-US")),
                Value = withdrawal
            });

            DataValidationErrors.ClearErrors(textBox);
        }

        public void OnViewWithdrawals()
        {
            ContentViewModel = WithdrawalViewModel;
        }

        public void OnAddInterestRate(object parameter)
        {
            var values = (object[])parameter;
            var valueTextBox = (TextBox)values[0];
            var rateTextBox = (TextBox)values[1];
            
            if (!double.TryParse(DepositMainViewModel.CurrentDependentValue, CultureInfo.InvariantCulture, out var depend_value))
            {
                DataValidationErrors.SetError(valueTextBox, new DataValidationException("Value must be a number."));
                return;
            }

            if (!double.TryParse(DepositMainViewModel.CurrentDependentRate, CultureInfo.InvariantCulture, out var depend_rate))
            {
                DataValidationErrors.SetError(rateTextBox, new DataValidationException("Rate must be a number."));
                return;
            }

            string depend_value_str;
            if (DepositMainViewModel.SelectedRateType == 1)
            {
                depend_value_str = depend_value.ToString("C", CultureInfo.GetCultureInfo("en-US"));
            }
            else
            {
                depend_value_str = ((int)depend_value).ToString(CultureInfo.GetCultureInfo("en-US"));
            }

            RatesViewModel.Items.Add(new DepositGridItem()
            {
                Id = RatesViewModel.Items.Count + 1,
                Parameter = depend_value_str,
                Value = depend_rate
            });

            DataValidationErrors.ClearErrors(valueTextBox);
            DataValidationErrors.ClearErrors(rateTextBox);
        }

        public void OnViewInterestRates()
        {
            ContentViewModel = RatesViewModel;
        }

        public void OnBackToMainView()
        {
            ContentViewModel = DepositMainViewModel;
        }

        public override void ForegroundBrushChangedAction(IBrush brush)
        {
            ForegroundBrush = brush;
            ForegroundBrushChanged?.Invoke(brush);
        }

        public override void FirstBackgroundBrushChangedAction(IBrush brush)
        {
            FirstBackgroundBrush = brush;
            FirstBackgroundBrushChanged?.Invoke(brush);
        }

        public override void SecondBackgroundBrushChangedAction(IBrush brush)
        {
            SecondBackgroundBrush = brush;
            SecondBackgroundBrushChanged?.Invoke(brush);
        }

        private void SetupColors(ViewModelBase viewModel)
        {
            this.ForegroundBrushChanged += viewModel.ForegroundBrushChangedAction;
            this.FirstBackgroundBrushChanged += viewModel.FirstBackgroundBrushChangedAction;
            this.SecondBackgroundBrushChanged += viewModel.SecondBackgroundBrushChangedAction;
        }
    }
}