using Avalonia;
using Avalonia.Diagnostics;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScientificCalculator.Services.Calculation;
using ScientificCalculator.Services.Logging;
using ScientificCalculator.ViewModels;
using ScientificCalculator.Views;

namespace ScientificCalculator;

public partial class App : Application
{
    public override void Initialize()
    {
        #if DEBUG
            this.AttachDevTools();
        #endif

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var logger = new FileCalculatorLogger(RotationPeriod.Hour);
            var calculation = new DllCalculationService();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(logger, calculation, calculation, calculation),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}