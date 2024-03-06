using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScientificCalculator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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

    // debug
    public void ExpandButtonClicked(object source, RoutedEventArgs args)
    {   
        // (source as TextBox).SelectedText
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