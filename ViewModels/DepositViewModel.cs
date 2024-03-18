using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data;
using ReactiveUI;
using ScientificCalculator.Models;

namespace ScientificCalculator.ViewModels
{
    public class DepositViewModel : ViewModelBase
    {
        private string? _depositAmount;
        public string? DepositAmount
        {
            get => _depositAmount;
            set 
            {
                CheckDouble(value);
                this.RaiseAndSetIfChanged(ref _depositAmount, value);
            }
        }
     
        private string? _term;
        public string? Term
        {
            get => _term;
            set 
            {
                if (string.IsNullOrEmpty(value) || !int.TryParse(value, CultureInfo.InvariantCulture, out var _))
                {
                    throw new DataValidationException("Term must be an integer value.");
                }
                this.RaiseAndSetIfChanged(ref _term, value);
            }
        }

        private DateTime _startTermDate;
        public DateTime StartTermDate
        {
            get => _startTermDate;
            set => this.RaiseAndSetIfChanged(ref _startTermDate, value);
        }

        private int _selectedTermType;
        public int SelectedTermType
        {
            get => _selectedTermType;
            set => this.RaiseAndSetIfChanged(ref _selectedTermType, value);
        }

        private int _selectedRateType;
        public int SelectedRateType
        {
            get => _selectedRateType;
            set => this.RaiseAndSetIfChanged(ref _selectedRateType, value);
        }

        private string? _taxRate;
        public string? TaxRate
        {
            get => _taxRate;
            set 
            {
                CheckDouble(value);
                this.RaiseAndSetIfChanged(ref _taxRate, value);
            }
        }

        private int _selectedPaymentPeriod;
        public int SelectedPaymentPeriod
        {
            get => _selectedPaymentPeriod;
            set => this.RaiseAndSetIfChanged(ref _selectedPaymentPeriod, value);
        }

        private bool _isInterestCapitalisationChecked;
        public bool IsInterestCapitalisationChecked
        {
            get => _isInterestCapitalisationChecked;
            set => this.RaiseAndSetIfChanged(ref _isInterestCapitalisationChecked, value);
        }

        private DateTime _currentReplenishmentDate;
        public DateTime CurrentReplenishmentDate
        {
            get => _currentReplenishmentDate;
            set => this.RaiseAndSetIfChanged(ref _currentReplenishmentDate, value);
        }

        private string? _currentReplenishment;
        public string? CurrentReplenishment
        {
            get => _currentReplenishment;
            set 
            {
                CheckDouble(value);
                this.RaiseAndSetIfChanged(ref _currentReplenishment, value);
            }
        }

        private DateTime _currentWithdrawalDate;
        public DateTime CurrentWithdrawalDate
        {
            get => _currentWithdrawalDate;
            set => this.RaiseAndSetIfChanged(ref _currentWithdrawalDate, value);
        }

        private string? _currentWithdrawal;
        public string? CurrentWithdrawal
        {
            get => _currentWithdrawal;
            set 
            {
                CheckDouble(value);
                this.RaiseAndSetIfChanged(ref _currentWithdrawal, value);
            }
        }

        public ObservableCollection<string> RateTypes { get; }
        public ObservableCollection<string> Periodicity { get; }
        public ObservableCollection<DepositReplenishment> Replenishments { get; }
        public ObservableCollection<DepositReplenishment> Withdrawals { get; }

        public DepositViewModel()
        {
            RateTypes = new ObservableCollection<string>
            {
                "Fixed",
                "Depends on the amount",
                "Depends on the term"
            };

            Periodicity = new ObservableCollection<string>
            {
                "Everyday",
                "Every week",
                "Once a month",
                "Once a quarter",
                "Once a year",
                "At the end of the term"
            };

            Replenishments = new ObservableCollection<DepositReplenishment>();
            Withdrawals = new ObservableCollection<DepositReplenishment>();

            StartTermDate = DateTime.Now;
            CurrentReplenishmentDate = DateTime.Now;
            CurrentWithdrawalDate = DateTime.Now;
        }

        public void OnAddReplenishment()
        {
            Replenishments.Add(new DepositReplenishment()
            {
                Id = Replenishments.Count + 1, 
                Date = DateOnly.FromDateTime(CurrentReplenishmentDate),
                Amount = double.Parse(CurrentReplenishment, CultureInfo.InvariantCulture)
            });
        }

        public void OnAddWithdrawal()
        {
            Withdrawals.Add(new DepositReplenishment()
            {
                Id = Withdrawals.Count + 1, 
                Date = DateOnly.FromDateTime(CurrentWithdrawalDate),
                Amount = double.Parse(CurrentWithdrawal, CultureInfo.InvariantCulture)
            });
        }

        private static void CheckDouble(string? str)
        {
            if (string.IsNullOrEmpty(str) || !double.TryParse(str, CultureInfo.InvariantCulture, out var _))
            {
                throw new DataValidationException("Value must be a number.");
            }
        }
    }
}