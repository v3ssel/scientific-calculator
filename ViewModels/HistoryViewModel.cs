using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;

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

        public HistoryViewModel()
        {
            _selectedExpression = new HistoryRecord();
            HistoryRecords = new ObservableCollection<HistoryRecord>();
        }

        public void DeleteHistoryRecord()
        {
            if (LastClickedRecord is not null && HistoryRecords.Contains(LastClickedRecord))
                HistoryRecords.Remove(LastClickedRecord);
        }

        public void DeleteAllHistory()
        {
            HistoryRecords.Clear();
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