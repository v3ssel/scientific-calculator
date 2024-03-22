using System.Collections.ObjectModel;
using ScientificCalculator.Models;

namespace ScientificCalculator.ViewModels;

public class DepositResultViewModel : ViewModelBase
{
    public DepositResult Result { get; set; }
    
    public DepositResultViewModel(DepositResult result)
    {
        Result = result;
    }
}
