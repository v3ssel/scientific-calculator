using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data;
using ReactiveUI;
using static ScientificCalculator.Utils.Utils;

namespace ScientificCalculator.ViewModels
{
    public class DepositMainViewModel : ViewModelBase
    {
        private string? _depositAmount;
        public string? DepositAmount
        {
            get => _depositAmount;
            set =>  this.RaiseAndSetIfChanged(ref _depositAmount, value);
        }
     
        private string? _term;
        public string? Term
        {
            get => _term;
            set => this.RaiseAndSetIfChanged(ref _term, value);
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

        // 0 - fixed
        // 1 - amount
        // 2 - days
        public int SelectedRateType
        {
            get => _selectedRateType;
            set => this.RaiseAndSetIfChanged(ref _selectedRateType, value);
        }

        private string? _fixedRate;
        public string? FixedRate
        {
            get => _fixedRate;
            set =>  this.RaiseAndSetIfChanged(ref _fixedRate, value);
        }

        private string? _dependentValueLabel;
        public string? DependentValueLabel
        {
            get => _dependentValueLabel;
            set => this.RaiseAndSetIfChanged(ref _dependentValueLabel, value);
        }

        private string? _currentDependentValue;
        public string? CurrentDependentValue
        {
            get => _currentDependentValue;
            set =>  this.RaiseAndSetIfChanged(ref _currentDependentValue, value);
        }

        private string? _currentDependentRate;
        public string? CurrentDependentRate
        {
            get => _currentDependentRate;
            set =>  this.RaiseAndSetIfChanged(ref _currentDependentRate, value);
        }

        private string? _taxRate;
        public string? TaxRate
        {
            get => _taxRate;
            set =>  this.RaiseAndSetIfChanged(ref _taxRate, value);
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
            set => this.RaiseAndSetIfChanged(ref _currentReplenishment, value);
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
            set => this.RaiseAndSetIfChanged(ref _currentWithdrawal, value);
        }

        public ObservableCollection<string> RateTypes { get; }
        public ObservableCollection<string> Periodicity { get; }

        public DepositMainViewModel()
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
                "Semiannually",
                "Once a year",
                "At the end of the term"
            };
            _selectedPaymentPeriod = 2;

            StartTermDate = DateTime.Now;
            CurrentReplenishmentDate = DateTime.Now;
            CurrentWithdrawalDate = DateTime.Now;
        }
    }
}
