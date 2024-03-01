using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ScientificCalculator.ViewModels;

namespace ScientificCalculator.Models;

public class SidebarButton
{
    public SidebarButton(ViewModelBase view_model_type, string icon_resource)
    {
        ViewModelType = view_model_type;
        Label = nameof(view_model_type).Replace("ViewModel", "");

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
    public ViewModelBase ViewModelType { get; set; }
    public StreamGeometry ButtonIcon { get; set; }
}
