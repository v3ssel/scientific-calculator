using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;
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

    private CalculatorViewModel _calculatorContent;
    public CalculatorViewModel CalculatorContent
    {
        get => _calculatorContent;
        set => this.RaiseAndSetIfChanged(ref _calculatorContent, value);
    }

    private GraphViewModel _graphContent;
    public GraphViewModel GraphContent
    {
        get => _graphContent;
        set => this.RaiseAndSetIfChanged(ref _graphContent, value);
    }

    private HistoryViewModel _historyContent;
    public HistoryViewModel HistoryContent
    {
        get => _historyContent;
        set => this.RaiseAndSetIfChanged(ref _historyContent, value);
    }

    private SettingsViewModel _settingsContent;
    public SettingsViewModel SettingsContent
    {
        get => _settingsContent;
        set => this.RaiseAndSetIfChanged(ref _settingsContent, value);
    }

    private AboutViewModel _aboutContent;
    public AboutViewModel AboutContent
    {
        get => _aboutContent;
        set => this.RaiseAndSetIfChanged(ref _aboutContent, value);
    }

    public MainWindowViewModel()
    {
        _graphContent = new GraphViewModel();
        _historyContent = new HistoryViewModel();
        _settingsContent = new SettingsViewModel();
        _aboutContent = new AboutViewModel();
        _calculatorContent = new CalculatorViewModel(_historyContent);
        
        _contentViewModel = _calculatorContent;

        HistoryContent.WhenAnyValue(x => x.SelectedExpression)
                      .Subscribe(HistoryValueSelectedAction);
    }

    public void CalculatorSidebarButtonClicked()
    {
        ContentViewModel = CalculatorContent;
    }

    public void GraphSidebarButtonClicked()
    {
        ContentViewModel = GraphContent;
    }
    
    public void HistorySidebarButtonClicked()
    {
        ContentViewModel = HistoryContent;
    }
    
    public void SettingsSidebarButtonClicked()
    {
        ContentViewModel = SettingsContent;
    }
    
    public void AboutSidebarButtonClicked()
    {
        ContentViewModel = AboutContent;
    }

    public void ExpandGraphBtnClicked()
    {
        IsSplitViewPaneOpen = !IsSplitViewPaneOpen;
    }

    private void HistoryValueSelectedAction(HistoryRecord x)
    {

        CalculatorContent.ExpressionInput = x.Expression;
        ContentViewModel = CalculatorContent;
    }
}
