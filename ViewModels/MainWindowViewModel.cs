using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Views;

namespace ScientificCalculator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _contentViewModel;

    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
    }

    private bool _isSplitViewPaneOpen;
    public bool IsSplitViewPaneOpen
    {
        get => _isSplitViewPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isSplitViewPaneOpen, value);
    }

    private SidebarButton _selectedSidebarButton;
    public SidebarButton SelectedSidebarButton
    {
        get => _selectedSidebarButton;
        set => this.RaiseAndSetIfChanged(ref _selectedSidebarButton, value);
    }

    private void SelectedSidebarButtonChanged(SidebarButton sidebarButton)
    {
        var view_model_instance = Activator.CreateInstance(sidebarButton.ViewModelType);    
        if (view_model_instance is null) return;

        ContentViewModel = (ViewModelBase)view_model_instance;
    }

    public ObservableCollection<SidebarButton> SidebarButtons { get; set; }

    public MainWindowViewModel()
    {
        _contentViewModel = new CalculatorViewModel();
        SidebarButtons = new ObservableCollection<SidebarButton>()
        {
            new(typeof(CalculatorViewModel), "CalculatorIcon"),
            new(typeof(GraphViewModel), "GraphIcon"),
            new(typeof(HistoryViewModel), "HistoryIcon"),
            new(typeof(SettingsViewModel), "SettingsIcon"),
            new(typeof(AboutViewModel), "AboutIcon"),
        };

        _selectedSidebarButton = SidebarButtons.First();

        this.WhenAnyValue(x => x.SelectedSidebarButton).Subscribe(SelectedSidebarButtonChanged);
    }

    public void ExpandGraphBtnClicked()
    {
        IsSplitViewPaneOpen = !IsSplitViewPaneOpen;
    }
}
