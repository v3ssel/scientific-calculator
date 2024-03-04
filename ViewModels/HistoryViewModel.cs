using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;
using ScientificCalculator.Models;

namespace ScientificCalculator.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
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
    }
}