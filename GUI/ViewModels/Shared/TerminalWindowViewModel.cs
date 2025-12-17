
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfaces.Handlers.Shared;

namespace GUI.ViewModels.Shared;

public partial class TerminalWindowViewModel(ITerminalQueryHandler terminalQueryHandler) : ViewModelBase
{
    private readonly ITerminalQueryHandler _terminalQueryHandler = terminalQueryHandler;

    [ObservableProperty]
    private string _queryString = "";

    [ObservableProperty]
    private string _outputString = "";

    [RelayCommand]
    public void ExecQuery()
    {
        Console.WriteLine("Query Executed:");
        Console.WriteLine($"{QueryString}");

        _terminalQueryHandler.HandleTerminalQuery(QueryString, out string outputString);
        OutputString = outputString;
        Console.WriteLine("Current Output:");
        Console.WriteLine($"{OutputString}");

    }
    
}