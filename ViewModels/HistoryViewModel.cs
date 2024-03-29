using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Saving;

namespace ScientificCalculator.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        #region  Properties

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

        public bool IsSaved { get; set; }

        #endregion

        private readonly ApplicationContext DbContext;

        public HistoryViewModel()
        {
            DbContext = new ApplicationContext();
            DbContext.Database.EnsureCreated();
        
            _selectedExpression = new HistoryRecord();
            HistoryRecords = new ObservableCollection<HistoryRecord>();
        }

        public void SetupFromDatabase()
        {
            DbContext.HistoryRecords.Load();
            HistoryRecords = new ObservableCollection<HistoryRecord>(DbContext.HistoryRecords.Local.OrderByDescending(x => x.CalculationTime));
        }

        public void OnCalculationComplete(CalculationStatus status, HistoryRecord record)
        {
            if (status == CalculationStatus.ERROR) return;

            HistoryRecords.Insert(0, record);

            Task.Run(async () =>
            {
                if (IsSaved)
                {
                    DbContext.HistoryRecords.Add(record);
                    await DbContext.SaveChangesAsync();
                }
            });
        }

        public void DeleteHistoryRecord()
        {
            if (LastClickedRecord is not null && HistoryRecords.Contains(LastClickedRecord))
            {
                HistoryRecords.Remove(LastClickedRecord);

                Task.Run(async () =>
                {
                    if (IsSaved)
                    {
                        DbContext.HistoryRecords.Remove(LastClickedRecord);
                        await DbContext.SaveChangesAsync();
                    }
                });
            }
        }

        public void DeleteAllHistory()
        {
            HistoryRecords.Clear();

            Task.Run(async () =>
            {
                if (IsSaved)
                {
                    await DbContext.HistoryRecords.ExecuteDeleteAsync();
                }
            });
        }
    }
}