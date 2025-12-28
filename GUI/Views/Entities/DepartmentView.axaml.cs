using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI.Views.Entities;

public partial class DepartmentView : UserControl
{
    public DepartmentView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}