using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Calculation;

namespace ScientificCalculator.ViewModels
{
    public class CreditViewModel : ViewModelBase
    {
        #region Events

        public delegate void UpdateForegroundBrush(IBrush brush);
        public event UpdateForegroundBrush? ForegroundBrushChanged;

        public delegate void UpdateBackgroundBrush(IBrush brush);
        public event UpdateBackgroundBrush? BackgroundBrushChanged;

        #endregion

        #region Properties

        private string? _amount;
        public string? Amount
        {
            get => _amount;
            set 
            {
                CheckDouble(value);
                this.RaiseAndSetIfChanged(ref _amount, value);
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

        private int _selectedTermType;
        public int SelectedTermType
        {
            get => _selectedTermType;
            set => this.RaiseAndSetIfChanged(ref _selectedTermType, value);
        }

        private string? _rate;
        public string? Rate
        {
            get => _rate;
            set 
            {
                CheckDouble(value);
                this.RaiseAndSetIfChanged(ref _rate, value);
            }
        }

        private bool _isAnnuityChecked;
        public bool IsAnnuityChecked
        {
            get => _isAnnuityChecked;
            set => this.RaiseAndSetIfChanged(ref _isAnnuityChecked, value);
        }

        private bool _isDifferChecked;
        public bool IsDifferChecked
        {
            get => _isDifferChecked;
            set => this.RaiseAndSetIfChanged(ref _isDifferChecked, value);
        }

        public ObservableCollection<CreditResult> CreditResults { get; private set; }

        private string? _monthlyPayment;
        public string? MonthlyPayment
        {
            get => _monthlyPayment;
            set => this.RaiseAndSetIfChanged(ref _monthlyPayment, value);
        }

        private string? _overpayment;
        public string? Overpayment
        {
            get => _overpayment;
            set => this.RaiseAndSetIfChanged(ref _overpayment, value);
        }

        private string? _totalPayout;
        public string? TotalPayout
        {
            get => _totalPayout;
            set => this.RaiseAndSetIfChanged(ref _totalPayout, value);
        }

        #endregion

        private readonly ICreditCalculationService CalculationService;

        public CreditViewModel(ICreditCalculationService calculationService)
        {
            CalculationService = calculationService;
            _isAnnuityChecked = true;
            _selectedTermType = 0;

            CreditResults = new ObservableCollection<CreditResult>();
        }

        public void CalculateButtonClicked(TextBox textBox)
        {
            try
            {
                CreditResults.Clear();

                if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(Rate) || string.IsNullOrEmpty(Term))
                {
                    throw new ArgumentException("Amount, rate and term must be filled in.");
                }

                int term = int.Parse(Term);
                if (SelectedTermType == 1)
                {
                    term *= 12;
                }

                var results = new List<CreditResult>();
                if (IsAnnuityChecked)
                {
                    results = CalculationService.CalculateMonthlyPaymentsAnnuity(double.Parse(Amount), double.Parse(Rate), term).ToList();
                }
                
                if (IsDifferChecked)
                {
                    results = CalculationService.CalculateMonthlyPaymentsDifferentiated(double.Parse(Amount), double.Parse(Rate), term).ToList();
                }

                results.ForEach(CreditResults.Add);
                MonthlyPayment = CreditResults.Last().Payment.ToString("C", CultureInfo.GetCultureInfo("en-US"));
                Overpayment = CreditResults.Last().Overpay.ToString("C", CultureInfo.GetCultureInfo("en-US"));
                TotalPayout = CreditResults.Last().Fullsum.ToString("C", CultureInfo.GetCultureInfo("en-US"));

                DataValidationErrors.ClearErrors(textBox);
            }
            catch (Exception e)
            {
                DataValidationErrors.SetError(textBox, new DataValidationException($"Error appeared during calculation.\n{e.Message}\nCheck your input."));
            }
        }

        public override void ForegroundBrushChangedAction(IBrush brush)
        {
            ForegroundBrush = brush;
            ForegroundBrushChanged?.Invoke(brush);
        }

        public override void SecondBackgroundBrushChangedAction(IBrush brush)
        {
            SecondBackgroundBrush = brush;
            BackgroundBrushChanged?.Invoke(brush);
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
