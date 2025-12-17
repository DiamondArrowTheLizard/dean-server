
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels.Shared;

public partial class TerminalWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _queryString = "";

    [ObservableProperty]
    private string _outputString = "";

    [RelayCommand]
    public void ExecQuery()
    {
        Console.WriteLine("Query Executed:");
        Console.WriteLine($"{QueryString}");
        OutputString = QueryString;
    }
    
}