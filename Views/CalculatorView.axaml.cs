using System;
using Avalonia.Controls;
using Avalonia.Input;
using ScientificCalculator.ViewModels;

namespace ScientificCalculator.Views;

public partial class CalculatorView : UserControl
{
    private CalculatorViewModel? ViewModel => this.DataContext as CalculatorViewModel;
    
    public CalculatorView()
    {
        InitializeComponent();
    }

    public void ExpressionGotFocusAction(object? sender, GotFocusEventArgs args)
    {
        if (ViewModel is not null)
            ViewModel.LastFocusToExpression = DateTime.Now.TimeOfDay;
    }

    public void XValueGotFocusAction(object? sender, GotFocusEventArgs args)
    {
        if (ViewModel is not null)
            ViewModel.LastFocusToX = DateTime.Now.TimeOfDay;
    }
}
