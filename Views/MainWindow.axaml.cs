using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using System.Diagnostics;
using System.Linq;

namespace ScientificCalculator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        // this.Transitions = new Transitions();
        InitializeComponent();

        // this.Transitions.Add(new DoubleTransition
        // {
        //     Property = Window.WidthProperty,
        //     Duration = TimeSpan.FromMilliseconds(5000)
        // });
    }
 
    public void PaneOpeningEvent(object source, CancelRoutedEventArgs args)
    {
        if (this.IsActive)
        {   
            var st = new Style(x => x.Is(typeof(Window)));

            var anim = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(500)
            };

            var start_kf = new KeyFrame
            {
                Cue = Cue.Parse("0%", System.Globalization.CultureInfo.InvariantCulture)
            };
            start_kf.Setters.Add(new Setter(MainWindow.WidthProperty, this.Width));

            var end_kf = new KeyFrame
            {
                Cue = Cue.Parse("100%", System.Globalization.CultureInfo.InvariantCulture)
            };
            end_kf.Setters.Add(new Setter(MainWindow.WidthProperty, this.Width += main_split.OpenPaneLength - main_split.CompactPaneLength));

            anim.Children.Add(start_kf);
            anim.Children.Add(end_kf);
            st.Animations.Add(anim);

            this.Styles.Add(st);

            this.Width += main_split.OpenPaneLength - main_split.CompactPaneLength;
        }
    }

    public void PaneClosingEvent(object source, CancelRoutedEventArgs args)
    {
        if (this.IsActive)
            this.Width -= main_split.OpenPaneLength - main_split.CompactPaneLength;
    }

    public void ExpandButtonClicked(object source, RoutedEventArgs args)
    {   
        // (source as TextBox).SelectedText
    }
}