using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI.Views.Help;

public partial class ManualView : UserControl
{
    public ManualView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}