using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using ReactiveUI;
using ScientificCalculator.Views;

namespace ScientificCalculator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;

    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
    }

    private bool _isSplitViewPaneOpen;
    public bool IsSplitViewPaneOpen
    {
        get => _isSplitViewPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isSplitViewPaneOpen, value);
    }

    public MainWindowViewModel()
    {
        _contentViewModel = new CalculatorViewModel();
    }

    public void ExpandGraphBtnClicked()
    {
        IsSplitViewPaneOpen = !IsSplitViewPaneOpen;
        // ContentViewModel = new GraphViewModel();
    }
}
