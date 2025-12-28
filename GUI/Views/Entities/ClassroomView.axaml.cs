using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI.Views.Entities;

public partial class ClassroomView : UserControl
{
    public ClassroomView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}