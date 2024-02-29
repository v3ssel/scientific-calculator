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

    public void SelectionChangedAction(object sender, SelectionChangedEventArgs routedEvent)
    {
        Debug.WriteLine("SELECTION CHANGED");
    }

    private void SelectedSidebarButtonChanged(SidebarButton sidebarButton)
    {
        Debug.WriteLine("SELECTION CHANGED");
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

        this.WhenAnyValue(x => x.SelectedSidebarButton).Subscribe(x => SelectedSidebarButtonChanged(x));
    }

    public void ExpandGraphBtnClicked()
    {
        IsSplitViewPaneOpen = !IsSplitViewPaneOpen;
        // ContentViewModel = new GraphViewModel();
    }
}

public class SidebarButton : INotifyPropertyChanged
{
    public SidebarButton(Type view_model_type, string icon_resource)
    {
        ViewModelType = view_model_type;
        Label = view_model_type.Name.Replace("ViewModel", "");

        if (Application.Current!.TryFindResource(icon_resource, out var icon)
            && icon is StreamGeometry icon_geometry)
        {
            ButtonIcon = icon_geometry;
        }
        else
        {
            ButtonIcon = new StreamGeometry();
        }

    }

    public string Label { get; set; }
    public Type ViewModelType { get; set; }
    public StreamGeometry ButtonIcon { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}