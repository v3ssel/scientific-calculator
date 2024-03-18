using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ScientificCalculator.ViewModels;

namespace ScientificCalculator.Views;

public partial class CreditWindow : ReactiveWindow<CreditViewModel>
{
    public CreditWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode) return;

        this.WhenActivated(action =>
        {
            ViewModel!.ForegroundBrushChanged += UpdateColumnHeaderForeground;
            ViewModel!.BackgroundBrushChanged += UpdateColumnHeaderBackground;

            UpdateColumnHeaderForeground(ViewModel!.ForegroundBrush);
            UpdateColumnHeaderBackground(ViewModel!.SecondBackgroundBrush);
        });
    }

    // avalonia doesnt support changing header colors property for now
    // so have to override dynamic resource value
    private void UpdateColumnHeaderForeground(IBrush brush)
    {
        this.Resources["DataGridColumnHeaderForegroundBrush"] = brush;
    }

    private void UpdateColumnHeaderBackground(IBrush brush)
    {
        this.Resources["DataGridColumnHeaderBackgroundBrush"] = brush;
    }
}