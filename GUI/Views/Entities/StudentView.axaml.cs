using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI.Views.Entities;

public partial class StudentView : UserControl
{
    public StudentView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}