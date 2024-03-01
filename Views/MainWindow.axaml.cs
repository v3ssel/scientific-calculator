using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;

namespace ScientificCalculator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void ExpandButtonClicked(object source, RoutedEventArgs args)
    {   
        // (source as TextBox).SelectedText
    }
}