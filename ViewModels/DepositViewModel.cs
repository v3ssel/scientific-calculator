using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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
        
        #region ViewSubmodels

        public DepositGridViewModel ReplenishmentViewModel { get; }
        public DepositGridViewModel WithdrawalViewModel { get; }
        public DepositGridViewModel RatesViewModel { get; }
        public DepositMainViewModel MainViewModel { get; }
        public DepositResultViewModel ResultViewModel { get; private set; }

        private ViewModelBase _contentViewModel;

        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        #endregion

        public ICommand OnAddReplenishmentCommand { get; }
        public ICommand OnAddWithdrawalCommand { get; }
        public ICommand OnAddInterestRateCommand { get; }
        public ICommand OnCalculateCommand { get; }

        private readonly IDepositCalculationService CalculationService;

        public DepositViewModel(IDepositCalculationService calculationService)
        {
            CalculationService = calculationService;

            MainViewModel = new DepositMainViewModel();
            _contentViewModel = MainViewModel;

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
                SecondColumnName = string.Empty,
                ThirdColumnName = "Rate"
            };

            ResultViewModel = new DepositResultViewModel(new DepositResult());

            OnAddReplenishmentCommand = ReactiveCommand.Create<TextBox>(OnAddReplenishment);
            OnAddWithdrawalCommand = ReactiveCommand.Create<TextBox>(OnAddWithdrawal);
            OnAddInterestRateCommand = ReactiveCommand.Create<object>(OnAddInterestRate);
            OnCalculateCommand = ReactiveCommand.Create<object>(OnCalculate);

            SetupColors(MainViewModel);
            SetupColors(ReplenishmentViewModel);
            SetupColors(WithdrawalViewModel);
            SetupColors(RatesViewModel);
            SetupColors(ResultViewModel);

            this.WhenAnyValue(x => x.MainViewModel.SelectedRateType).Subscribe(SelectedRateTypeChanged);
        }

        public void OnCalculate(object parameters)
        {
            try
            {
                var (error, depAmount, depTerm, depFixedRate, depTaxRate) = ReadValues(parameters);
                if (error) return;
                
                var daysInTerm = GetTermInDays(depTerm);
                var period = GetPeriodicity(daysInTerm);

                var (replenishDays, replenishAmount) = GetReplenishments(daysInTerm);
                var (rates, rate_dependence) = GetInterestRates(depFixedRate);

                var p = new DepositParams
                {
                    start_amount = depAmount,
                    term_in_days = daysInTerm,
                    term_begin_day = MainViewModel.StartTermDate.Day,
                    term_begin_month = MainViewModel.StartTermDate.Month,
                    term_begin_year = MainViewModel.StartTermDate.Year,
                    tax_rate = depTaxRate,
                    periodicity = period,
                    capitalization = MainViewModel.IsInterestCapitalisationChecked,

                    days_of_replenishments = Marshal.AllocHGlobal(replenishDays.Count() * sizeof(int)),
                    amount_of_replenishments = Marshal.AllocHGlobal(replenishAmount.Count() * sizeof(double)),
                    count_of_replenishments = replenishDays.Count(),

                    rate_type = MainViewModel.SelectedRateType,
                    rates = Marshal.AllocHGlobal(rates.Count() * sizeof(double)),
                    rate_dependence_values = Marshal.AllocHGlobal(rate_dependence.Count() * sizeof(double)),
                    count_of_rates = rates.Count()
                };

                Marshal.Copy(replenishDays.ToArray(),   0, p.days_of_replenishments,   replenishDays.Count());
                Marshal.Copy(replenishAmount.ToArray(), 0, p.amount_of_replenishments, replenishAmount.Count());
                Marshal.Copy(rates.ToArray(),           0, p.rates,                    rates.Count());
                Marshal.Copy(rate_dependence.ToArray(), 0, p.rate_dependence_values,   rate_dependence.Count());

                var result = CalculationService.CalculateDepositIncome(p);

                Marshal.FreeHGlobal(p.days_of_replenishments);
                Marshal.FreeHGlobal(p.amount_of_replenishments);
                Marshal.FreeHGlobal(p.rates);
                Marshal.FreeHGlobal(p.rate_dependence_values);

                ResultViewModel.Result = result;
                ContentViewModel = ResultViewModel;
            }
            catch (Exception e)
            {
                var tBox = ((object[])parameters)[0] as TextBox;
                DataValidationErrors.SetError(tBox!, new DataValidationException($"Error occured during calculation.\n{e.Message}\nPlease, check your input."));
            }
        }

        public void OnAddReplenishment(TextBox textBox)
        {
            if (!double.TryParse(MainViewModel.CurrentReplenishment, CultureInfo.InvariantCulture, out var replenishment))
            {
                DataValidationErrors.SetError(textBox, new DataValidationException("Amount must be a number."));
                return;
            }

            ReplenishmentViewModel.Items.Add(new DepositGridItem()
            {
                Id = ReplenishmentViewModel.Items.Count + 1, 
                Parameter = MainViewModel.CurrentReplenishmentDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("en-US")),
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
            if (!double.TryParse(MainViewModel.CurrentWithdrawal, CultureInfo.InvariantCulture, out var withdrawal))
            {
                DataValidationErrors.SetError(textBox, new DataValidationException("Amount must be a number."));
                return;
            }

            WithdrawalViewModel.Items.Add(new DepositGridItem()
            {
                Id = WithdrawalViewModel.Items.Count + 1, 
                Parameter = MainViewModel.CurrentWithdrawalDate.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("en-US")),
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
            
            if (!double.TryParse(MainViewModel.CurrentDependentValue, CultureInfo.InvariantCulture, out var depend_value))
            {
                DataValidationErrors.SetError(valueTextBox, new DataValidationException("Value must be a number."));
                return;
            }

            if (!double.TryParse(MainViewModel.CurrentDependentRate, CultureInfo.InvariantCulture, out var depend_rate))
            {
                DataValidationErrors.SetError(rateTextBox, new DataValidationException("Rate must be a number."));
                return;
            }

            string depend_value_str;
            if (MainViewModel.SelectedRateType == 1)
            {
                depend_value_str = depend_value.ToString("C", CultureInfo.GetCultureInfo("en-US"));
            }
            else
            {
                depend_value_str = ((int)depend_value).ToString("N", CultureInfo.GetCultureInfo("en-US"));
            }

            var elem = RatesViewModel.Items.FirstOrDefault(x => x.Parameter == depend_value_str);

            if (elem is null)
            {
                RatesViewModel.Items.Add(new DepositGridItem()
                {
                    Id = RatesViewModel.Items.Count + 1,
                    Parameter = depend_value_str,
                    Value = depend_rate
                });
            }
            else
            {
                elem.Value = depend_rate;
            }

            DataValidationErrors.ClearErrors(valueTextBox);
            DataValidationErrors.ClearErrors(rateTextBox);
        }

        public void OnViewInterestRates()
        {
            ContentViewModel = RatesViewModel;
        }

        public void OnBackToMainView()
        {
            ContentViewModel = MainViewModel;
        }

        private void SelectedRateTypeChanged(int x)
        {
            if (x == 1)
            {
                MainViewModel.DependentValueLabel = "Amount with which the rate is valid";
                RatesViewModel.SecondColumnName = "Amount";
            }
            if (x == 2)
            {
                MainViewModel.DependentValueLabel = "Number of the day from which the rate is valid";
                RatesViewModel.SecondColumnName = "Day Number";
            }

            RatesViewModel.Items.Clear();
        }

        private (bool, double, int, double, double) ReadValues(object parameters)
        {
            var values = (object[])parameters;
            var amountTextBox = (TextBox)values[0];
            var termTextBox = (TextBox)values[1];
            var fixedRateTextBox = (TextBox)values[2];
            var taxRateTextBox = (TextBox)values[3];

            bool error = false;
            if (!double.TryParse(MainViewModel.DepositAmount, CultureInfo.InvariantCulture, out var depAmount))
            {
                DataValidationErrors.SetError(amountTextBox, new DataValidationException("Amount must be a number."));
                error = true;
            }

            if (!int.TryParse(MainViewModel.Term, CultureInfo.InvariantCulture, out var depTerm))
            {
                DataValidationErrors.SetError(termTextBox, new DataValidationException("Term must be an integer number."));
                error = true;
            }

            if (!double.TryParse(MainViewModel.FixedRate, CultureInfo.InvariantCulture, out var depFixedRate) && MainViewModel.SelectedRateType == 0)
            {
                DataValidationErrors.SetError(fixedRateTextBox, new DataValidationException("Rate must be a number."));
                error = true;
            }

            if (!double.TryParse(MainViewModel.TaxRate, CultureInfo.InvariantCulture, out var depTaxRate))
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

        private int GetTermInDays(int term)
        {
            return MainViewModel.SelectedTermType switch
            {
                0 => (MainViewModel.StartTermDate.AddDays(term) - MainViewModel.StartTermDate).Days,
                1 => (MainViewModel.StartTermDate.AddMonths(term) - MainViewModel.StartTermDate).Days,
                2 => (MainViewModel.StartTermDate.AddYears(term) - MainViewModel.StartTermDate).Days,
                _ => (MainViewModel.StartTermDate.AddDays(term) - MainViewModel.StartTermDate).Days,
            };

        }

        private int GetPeriodicity(int total_term)
        {
            return MainViewModel.SelectedPaymentPeriod switch
            {
                0 => 1,
                1 => 7,
                2 => 30,
                3 => 365 / 4,
                4 => 365 / 2,
                5 => 365,
                6 => total_term,
                _ => 1,
            };
        }

        private (IEnumerable<int>, IEnumerable<double>) GetReplenishments(int total_term)
        {
            var reps = ReplenishmentViewModel
                    .Items
                    .Concat(WithdrawalViewModel.Items)
                    .Select(x => new
                            {
                                Parameter = DateTime.Parse(x.Parameter),
                                x.Value
                            })
                    .OrderBy(x => x.Parameter)
                    .GroupBy(x => x.Parameter)
                    .Select(x =>  x.Aggregate((acc, x) => 
                                new
                                {
                                    acc.Parameter,
                                    Value = acc.Value + x.Value
                                }
                            ))
                    .Where(x => x.Value != 0 &&
                                x.Parameter >= MainViewModel.StartTermDate &&
                                x.Parameter <= MainViewModel.StartTermDate.AddDays(total_term));

            var replenishDays = reps.Select(x => (x.Parameter - MainViewModel.StartTermDate).Days);
            var replenishAmount = reps.Select(x => x.Value);

            return (replenishDays, replenishAmount);
        }

        private (IEnumerable<double>, IEnumerable<double>) GetInterestRates(double fixed_rate)
        {
            var sorted_rates = RatesViewModel.Items
                                             .Select(x => new
                                                    {
                                                        Parameter = double.Parse(x.Parameter,
                                                                                 MainViewModel.SelectedRateType == 1 
                                                                                                ? NumberStyles.Currency
                                                                                                : NumberStyles.Number, 
                                                                                 CultureInfo.GetCultureInfo("en-US")),
                                                        x.Value
                                                    })
                                             .OrderBy(x => x.Parameter)
                                             .DistinctBy(x => x.Parameter);

            var rates = MainViewModel.SelectedRateType != 0
                            ? sorted_rates.Select(x => x.Value)
                            : new List<double>() { fixed_rate };
                            
            var rate_dependence = MainViewModel.SelectedRateType != 0 
                        ? sorted_rates.Select(x => x.Parameter)
                        : new List<double>() { 0 };
            
            return (rates, rate_dependence);
        }
    }
}