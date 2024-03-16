using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using ScientificCalculator.Models;
using ScientificCalculator.ViewModels;

namespace ScientificCalculator.Views;

public partial class HistoryView : UserControl
{
    private HistoryViewModel? ViewModel => this.DataContext as HistoryViewModel;
    public HistoryView()
    {
        InitializeComponent();

        listBox.AddHandler(PointerPressedEvent, ItemPressed, RoutingStrategies.Tunnel);
    }

    public void ItemPressed(object? sender, PointerPressedEventArgs args)
    {
        var point = args.GetCurrentPoint(sender as Control);
        
        if (point.Properties.IsRightButtonPressed)
        {
            args.Handled = true;

            if (ViewModel is null)
            {
                Avalonia.Logging.Logger.Sink?.Log(Avalonia.Logging.LogEventLevel.Error, "HistoryView", ViewModel, "ViewModel is null");
                
                return;
            }

            if (point.Pointer.Captured is not ContentPresenter captured)
            {
                Avalonia.Logging.Logger.Sink?.Log(Avalonia.Logging.LogEventLevel.Error, "HistoryView", point.Pointer.Captured, "Pointer.Captured is null");

                return;
            }

            ViewModel.LastClickedRecord = captured.Content as HistoryRecord;            
        }
    }
}
