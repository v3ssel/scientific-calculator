using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;

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
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }
     
        private string? _term;
        public string? Term
        {
            get => _term;
            set => this.RaiseAndSetIfChanged(ref _term, value);
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
            set => this.RaiseAndSetIfChanged(ref _rate, value);
        }

        private bool _isAnnuityChecked;
        public bool IsAnnuityChecked
        {
            get => _isAnnuityChecked;
            set => this.RaiseAndSetIfChanged(ref _isAnnuityChecked, value);
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

        public CreditViewModel()
        {
            _isAnnuityChecked = true;
            _selectedTermType = 0;

            CreditResults = new ObservableCollection<CreditResult>();
        }

        public void CalculateButtonClicked()
        {
            CreditResults.Clear();

            for (int i = 0; i < 20; i++)
            {
                CreditResults.Add(new CreditResult() { Month = i, Payment = i + 1, Fullsum = i + 3, Overpay = i + i });
            }

            MonthlyPayment = CreditResults.Last().Payment.ToString();
            Overpayment = CreditResults.Last().Overpay.ToString();
            TotalPayout = CreditResults.Last().Fullsum.ToString();
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
    }
}