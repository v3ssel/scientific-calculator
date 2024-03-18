using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Calculation;
using ScientificCalculator.Services.Logging;

namespace ScientificCalculator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Events

    public delegate void ShowCreditWindow(object? context);
    public event ShowCreditWindow? ShowCreditWindowEvent;

    public delegate void ShowDepositWindow(object? context);
    public event ShowDepositWindow? ShowDepositWindowEvent;

    #endregion

    #region Properties

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

    private CreditViewModel _creditContent;
    public CreditViewModel CreditContent
    {
        get => _creditContent;
        set => this.RaiseAndSetIfChanged(ref _creditContent, value);
    }

    private DepositViewModel _depositContent;
    public DepositViewModel DepositContent
    {
        get => _depositContent;
        set => this.RaiseAndSetIfChanged(ref _depositContent, value);
    }

    #endregion

    private readonly ICalculatorLogger Logger;
    private readonly ICalculationService CalculationService;

    public MainWindowViewModel(ICalculatorLogger logger, ICalculationService calculationService, ICreditCalculationService creditService)
    {
        Logger = logger;
        CalculationService = calculationService;

        _calculatorContent = new CalculatorViewModel(CalculationService);
        _graphContent = new GraphViewModel(CalculationService);
        _historyContent = new HistoryViewModel();
        _settingsContent = new SettingsViewModel();
        _aboutContent = new AboutViewModel();
        _creditContent = new CreditViewModel(creditService);
        _depositContent = new DepositViewModel();

        _contentViewModel = _calculatorContent;

        CalculatorContent.CalculationCompleteEvent += OnCalculationComplete;
        CalculatorContent.CalculationCompleteEvent += HistoryContent.OnCalculationComplete;

        GraphContent.GraphPlottedEvent += OnCalculationComplete;
        GraphContent.GraphPlottedEvent += HistoryContent.OnCalculationComplete;

        HistoryContent.SetupFromDatabase();
        HistoryContent.WhenAnyValue(x => x.SelectedExpression).Subscribe(HistoryValueSelectedAction);

        SetupColors(this);
        SetupColors(CalculatorContent);
        SetupColors(GraphContent);
        SetupColors(HistoryContent);
        SetupColors(AboutContent);
        SetupColors(CreditContent);
        SetupColors(DepositContent);

        SettingsContent.LogEnableChangedEvent += LogEnableChangedAction;
        SettingsContent.SaveHistoryEnableChangedEvent += SaveHistoryChangedAction;

        SettingsContent.SetupFromDatabase();
    }

    #region EventHandlers

    public void SaveHistoryChangedAction(bool enable)
    {
        HistoryContent.IsSaved = enable;
    }

    public void LogEnableChangedAction(bool enable)
    {
        Logger.Enabled = enable;
    }

    public void RotationPeriodChangedAction(RotationPeriod period)
    {
        if (Logger is ICalculatorLoggerWithRotation logger)
            logger.RotationPeriod = period;
    }

    public void OnCalculationComplete(CalculationStatus status, HistoryRecord record)
    {
        Task.Run(async () =>
            await Logger.LogAsync(status, record)
        );
    }

    private void SetupColors(ViewModelBase viewModel)
    {
        SettingsContent.ForegroundBrushChangedEvent += viewModel.ForegroundBrushChangedAction;
        SettingsContent.FirstBackgroundBrushChangedEvent += viewModel.FirstBackgroundBrushChangedAction;
        SettingsContent.SecondBackgroundBrushChangedEvent += viewModel.SecondBackgroundBrushChangedAction;
    }

    #endregion

    #region Buttons

    public void CalculatorSidebarButtonClicked()
    {
        if (!string.IsNullOrEmpty(GraphContent.ExpressionInput) || ContentViewModel is GraphViewModel)
        {
            CalculatorContent.ExpressionInput = GraphContent.ExpressionInput;
            CalculatorContent.ExpressionInputCaretIndex = GraphContent.ExpressionInput.Length;
        }

        ContentViewModel = CalculatorContent;
    }

    public void GraphSidebarButtonClicked()
    {
        if (!string.IsNullOrEmpty(CalculatorContent.ExpressionInput) || ContentViewModel is CalculatorViewModel)
            GraphContent.ExpressionInput = CalculatorContent.ExpressionInput;

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

    public void CreditBtnClicked()
    {
        ShowCreditWindowEvent?.Invoke(CreditContent);
    }

    public void DepositBtnClicked()
    {
        ShowDepositWindowEvent?.Invoke(DepositContent);
    }

    public void ExpandGraphBtnClicked()
    {
        IsSplitViewPaneOpen = !IsSplitViewPaneOpen;
    }

    private void HistoryValueSelectedAction(HistoryRecord? x)
    {
        if (x is null) return;

        CalculatorContent.ExpressionInput = x.Expression;
        CalculatorContent.XValue = x.XValue ?? string.Empty;
        ContentViewModel = CalculatorContent;
    }

    #endregion
}
