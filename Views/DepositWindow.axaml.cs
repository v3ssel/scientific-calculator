using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ScientificCalculator.ViewModels;

namespace ScientificCalculator.Views;

public partial class DepositWindow : ReactiveWindow<DepositViewModel>
{
    public DepositWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode) return;

        this.WhenActivated(action =>
        {
            ViewModel!.ForegroundBrushChanged += UpdateColumnHeaderForeground;
            ViewModel!.SecondBackgroundBrushChanged += UpdateColumnHeaderBackground;

            UpdateColumnHeaderForeground(ViewModel!.ForegroundBrush);
            UpdateColumnHeaderBackground(ViewModel!.SecondBackgroundBrush);
        });
    }

    // avalonia doesnt support changing data grid header colors property for now
    // so i have to override dynamic resource value
    private void UpdateColumnHeaderForeground(IBrush brush)
    {
        this.Resources["DataGridColumnHeaderForegroundBrush"] = brush;
    }

    private void UpdateColumnHeaderBackground(IBrush brush)
    {
        this.Resources["DataGridColumnHeaderBackgroundBrush"] = brush;
    }
}