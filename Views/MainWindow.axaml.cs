using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using ReactiveUI;
using ScientificCalculator.ViewModels;
using System;

namespace ScientificCalculator.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(action =>
        {
            ViewModel!.ShowCreditWindowEvent += DoShowCreditDialog;
            ViewModel!.ShowDepositWindowEvent += DoShowDepositDialog;
        });
    }

    private void DoShowCreditDialog(object? context)
    {
        var dialog = new CreditWindow { DataContext = context };
        dialog.Show();
    }

    private void DoShowDepositDialog(object? context)
    {
        var dialog = new DepositWindow { DataContext = context };
        dialog.Show();
    }
 
    public void PaneOpeningEvent(object source, CancelRoutedEventArgs args)
    {
        if (this.IsActive)
        {
            StartAnimation(MainWindow.WidthProperty, this.Width, this.Width + (main_split.OpenPaneLength - main_split.CompactPaneLength));
        }
    }

    public void PaneClosingEvent(object source, CancelRoutedEventArgs args)
    {
        if (this.IsActive)
        {
            StartAnimation(MainWindow.WidthProperty, this.Width, this.Width - (main_split.OpenPaneLength - main_split.CompactPaneLength));
        }
    }

    private void StartAnimation(AvaloniaProperty property, object initial_value, object desired_value)
    {
        var anim = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(50)
        };

        var start_kf = new KeyFrame
        {
            Cue = new Cue(0)
        };
        start_kf.Setters.Add(new Setter(property, initial_value));

        var end_kf = new KeyFrame
        {
            Cue = new Cue(1)
        };
        end_kf.Setters.Add(new Setter(property, desired_value));

        anim.Children.Add(start_kf);
        anim.Children.Add(end_kf);

        anim.RunAsync(this);
    }
}