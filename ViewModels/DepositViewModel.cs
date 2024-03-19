using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Calculation;

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
        public ICommand OnCalculateCommand { get; }

        private readonly IDepositCalculationService CalculationService;

        public DepositViewModel(IDepositCalculationService calculationService)
        {
            CalculationService = calculationService;

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
            OnCalculateCommand = ReactiveCommand.Create<object>(OnCalculate);

            SetupColors(DepositMainViewModel);
            SetupColors(ReplenishmentViewModel);
            SetupColors(WithdrawalViewModel);
            SetupColors(RatesViewModel);

            this.WhenAnyValue(x => x.DepositMainViewModel.SelectedRateType).Subscribe(SelectedRateTypeChanged);
        }

        public void OnCalculate(object parameters)
        {
            var (error, depAmount, depTerm, depFixedRate, depTaxRate) = CheckValues(parameters);
            if (error) return;

            var daysInTerm = DepositMainViewModel.SelectedTermType switch
            {
                0 => (DepositMainViewModel.StartTermDate.AddDays(depTerm) - DepositMainViewModel.StartTermDate).Days,
                1 => (DepositMainViewModel.StartTermDate.AddMonths(depTerm) - DepositMainViewModel.StartTermDate).Days,
                2 => (DepositMainViewModel.StartTermDate.AddYears(depTerm) - DepositMainViewModel.StartTermDate).Days,
                _ => (DepositMainViewModel.StartTermDate.AddDays(depTerm) - DepositMainViewModel.StartTermDate).Days,
            };

            var period = DepositMainViewModel.SelectedPaymentPeriod switch
            {
                0 => 1,
                1 => 7,
                2 => 30,
                3 => 365 / 4,
                4 => 365 / 2,
                5 => 365,
                6 => daysInTerm,
                _ => 1,
            };

            bool capitalism = DepositMainViewModel.IsInterestCapitalisationChecked;

            var rates = RatesViewModel.Items.OrderBy(x => x.Parameter);

            var reps = ReplenishmentViewModel
                    .Items
                    .Concat(WithdrawalViewModel.Items)
                    .OrderBy(x => x.Parameter)
                    .GroupBy(x => x.Parameter)
                    .Select(x =>  x.Aggregate((acc, x) => 
                                new()
                                {
                                    Parameter = acc.Parameter,
                                    Value = acc.Value + x.Value
                                }
                            ))
                    .Where(x => x.Value != 0);

            var replenishDays = new List<int>();
            foreach (var rep in reps)
            {
                replenishDays.Add((DepositMainViewModel.StartTermDate - DateTime.Parse(rep.Parameter)).Days);
            }
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
                Value = -withdrawal
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

        private void SelectedRateTypeChanged(int x)
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
        }

        private (bool, double, int, double, double) CheckValues(object parameters)
        {
            var values = (object[])parameters;
            var amountTextBox = (TextBox)values[0];
            var termTextBox = (TextBox)values[1];
            var fixedRateTextBox = (TextBox)values[2];
            var taxRateTextBox = (TextBox)values[3];

            bool error = false;
            if (!double.TryParse(DepositMainViewModel.DepositAmount, CultureInfo.InvariantCulture, out var depAmount))
            {
                DataValidationErrors.SetError(amountTextBox, new DataValidationException("Amount must be a number."));
                error = true;
            }

            if (!int.TryParse(DepositMainViewModel.Term, CultureInfo.InvariantCulture, out var depTerm))
            {
                DataValidationErrors.SetError(termTextBox, new DataValidationException("Term must be an integer number."));
                error = true;
            }

            if (!double.TryParse(DepositMainViewModel.FixedRate, CultureInfo.InvariantCulture, out var depFixedRate))
            {
                DataValidationErrors.SetError(fixedRateTextBox, new DataValidationException("Rate must be a number."));
                error = true;
            }

            if (!double.TryParse(DepositMainViewModel.TaxRate, CultureInfo.InvariantCulture, out var depTaxRate))
            {
                DataValidationErrors.SetError(taxRateTextBox, new DataValidationException("Tax Rate must be a number."));
                error = true;
            }

            return (error, depAmount, depTerm, depFixedRate, depTaxRate);
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