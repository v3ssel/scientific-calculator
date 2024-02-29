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
        // this.DataContext = new
        // if (main_split.IsPaneOpen)
        // {
        //     main_split.IsPaneOpen = false;
        //     expand_btn.Content = "<";
        //     expand_btn.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        //     expand_btn.Margin = Avalonia.Thickness.Parse("0 10 0 0");
        //     // (source as Button)?
        // }
        // else
        // {
        //     main_split.IsPaneOpen = true;
        //     expand_btn.Content = ">";
        //     expand_btn.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        //     expand_btn.Margin = Avalonia.Thickness.Parse("10 10 0 0");
        // }
    }
}