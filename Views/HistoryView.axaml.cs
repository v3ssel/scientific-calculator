using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
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
            if (ViewModel is null)
            {
                throw new NullReferenceException("ViewModel is null.");
            }

            if (point.Pointer.Captured is not ContentPresenter captured)
            {
                throw new NullReferenceException("point.Pointer.Captured is null or not ContentPresenter type.");
            }

            ViewModel.LastClickedRecord = captured.Content as HistoryRecord;
            
            args.Handled = true;
            return;
        }
    }
}
