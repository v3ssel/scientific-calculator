using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ScientificCalculator.Models;

namespace ScientificCalculator.ViewModels;

public class DepositGridViewModel : ViewModelBase
{
    public ObservableCollection<DepositGridItem> Items { get; set; }

    public string? Title { get; set; }
    public string? FirstColumnName { get; set; }
    public string? SecondColumnName { get; set; }
    public string? ThirdColumnName { get; set; }

    public DepositGridViewModel()
    {
        Items = new ObservableCollection<DepositGridItem>();
    }
}
