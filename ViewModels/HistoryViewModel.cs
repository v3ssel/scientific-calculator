using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Saving;

namespace ScientificCalculator.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private IBrush _foregroundBrush = Brushes.Black;
        public IBrush ForegroundBrush
        {
            get => _foregroundBrush;
            set => this.RaiseAndSetIfChanged(ref _foregroundBrush, value);
        }

        private IBrush _firstBackgroundBrush = Brushes.White;
        public IBrush FirstBackgroundBrush
        {
            get => _firstBackgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _firstBackgroundBrush, value);
        }

        private IBrush _secondBackgroundBrush = Brushes.Silver;
        public IBrush SecondBackgroundBrush
        {
            get => _secondBackgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _secondBackgroundBrush, value);
        }

        private HistoryRecord? _lastClickedRecord;
        public HistoryRecord? LastClickedRecord
        {
            get => _lastClickedRecord;
            set => this.RaiseAndSetIfChanged(ref _lastClickedRecord, value);
        }

        private HistoryRecord _selectedExpression; 
        public HistoryRecord SelectedExpression
        {
            get => _selectedExpression;
            set => this.RaiseAndSetIfChanged(ref _selectedExpression, value);
        }

        public ObservableCollection<HistoryRecord> HistoryRecords { get; set; }

        private ApplicationContext DbContext;

        public HistoryViewModel()
        {
            DbContext = new ApplicationContext();
            DbContext.Database.EnsureCreated();

            _selectedExpression = new HistoryRecord();
            HistoryRecords = new ObservableCollection<HistoryRecord>();
        }

        public void OnCalculationComplete(bool error, HistoryRecord record)
        {
            if (error) return;

            HistoryRecords.Insert(0, record);

            Task.Run(async () =>
            {
                DbContext.HistoryRecords.Add(record);
                await DbContext.SaveChangesAsync();
            });
        }

        public void DeleteHistoryRecord()
        {
            if (LastClickedRecord is not null && HistoryRecords.Contains(LastClickedRecord))
            {
                HistoryRecords.Remove(LastClickedRecord);

                Task.Run(async () =>
                {
                    DbContext.HistoryRecords.Remove(LastClickedRecord);
                    await DbContext.SaveChangesAsync();
                });
            }
        }

        public void DeleteAllHistory()
        {
            HistoryRecords.Clear();

            Task.Run(async () =>
                await DbContext.HistoryRecords.ExecuteDeleteAsync()
            );
        }

        public void ForegroundBrushChangedAction(IBrush brush)
        {
            ForegroundBrush = brush;
        }

        public void FirstBackgroundBrushChangedAction(IBrush brush)
        {
            FirstBackgroundBrush = brush;
        }

        public void SecondBackgroundBrushChangedAction(IBrush brush)
        {
            SecondBackgroundBrush = brush;
        }
    }
}