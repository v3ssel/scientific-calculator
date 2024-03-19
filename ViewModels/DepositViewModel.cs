using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Views;
using static ScientificCalculator.Utils.Utils;

namespace ScientificCalculator.ViewModels
{
    public class DepositViewModel : ViewModelBase
    {
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
                SecondColumnName = "Day/Amount", // change
                ThirdColumnName = "Rate"
            };

            OnAddReplenishmentCommand = ReactiveCommand.Create<TextBox>(OnAddReplenishment);
            OnAddWithdrawalCommand = ReactiveCommand.Create<TextBox>(OnAddWithdrawal);
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

        public void OnBackToMainView()
        {
            ContentViewModel = DepositMainViewModel;
        }
    }
}