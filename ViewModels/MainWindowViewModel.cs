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
using Avalonia.Data;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Logging;
using ScientificCalculator.Services.Saving;
using ScientificCalculator.Views;

namespace ScientificCalculator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Properties

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

    private bool _isSplitViewPaneOpen;
    public bool IsSplitViewPaneOpen
    {
        get => _isSplitViewPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isSplitViewPaneOpen, value);
    }

    #endregion

    #region ContentViewModels

    private ViewModelBase _contentViewModel;

    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
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

    #endregion

    private readonly ICalculatorLogger Logger;

    public MainWindowViewModel(ICalculatorLogger logger)
    {
        Logger = logger;
        // logger.Enabled = false;

        _graphContent = new GraphViewModel();
        _historyContent = new HistoryViewModel();
        _settingsContent = new SettingsViewModel();
        _aboutContent = new AboutViewModel();
        _calculatorContent = new CalculatorViewModel();
        
        _contentViewModel = _calculatorContent;

        CalculatorContent.CalculationCompleteEvent += OnCalculationComplete;
        CalculatorContent.CalculationCompleteEvent += HistoryContent.OnCalculationComplete;

        HistoryContent.WhenAnyValue(x => x.SelectedExpression).Subscribe(HistoryValueSelectedAction);

        SettingsContent.ForegroundBrushChangedEvent += ForegroundBrushChangedAction;
        SettingsContent.ForegroundBrushChangedEvent += CalculatorContent.ForegroundBrushChangedAction;
        SettingsContent.ForegroundBrushChangedEvent += HistoryContent.ForegroundBrushChangedAction;
        
        SettingsContent.FirstBackgroundBrushChangedEvent += FirstBackgroundBrushChangedAction;
        SettingsContent.FirstBackgroundBrushChangedEvent += CalculatorContent.FirstBackgroundBrushChangedAction;
        SettingsContent.FirstBackgroundBrushChangedEvent += HistoryContent.FirstBackgroundBrushChangedAction;

        SettingsContent.SecondBackgroundBrushChangedEvent += SecondBackgroundBrushChangedAction;
        SettingsContent.SecondBackgroundBrushChangedEvent += CalculatorContent.SecondBackgroundBrushChangedAction;
        SettingsContent.SecondBackgroundBrushChangedEvent += HistoryContent.SecondBackgroundBrushChangedAction;
    }

    #region EventHandlers

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

    public void OnCalculationComplete(bool error, HistoryRecord record)
    {
        Task.Run(async () =>
            await Logger.LogAsync(error ? LogLevel.ERROR : LogLevel.CALCULATED, record)
        );
    }

    #endregion

    #region Buttons

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

    private void HistoryValueSelectedAction(HistoryRecord? x)
    {
        if (x is null) return;
        
        CalculatorContent.ExpressionInput = x.Expression;
        ContentViewModel = CalculatorContent;
    }

    #endregion
}
