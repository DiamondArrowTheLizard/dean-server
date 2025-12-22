using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Interfaces.Models;

namespace GUI.ViewModels.Shared;

public partial class WelcomeScreenViewModel : ViewModelBase
{
    private readonly IConnectionInfo _connectionInfo;

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    [ObservableProperty]
    private string _username = string.Empty;

    public WelcomeScreenViewModel(IConnectionInfo connectionInfo)
    {
        _connectionInfo = connectionInfo;
        GetUsernameAndSetMessage(_connectionInfo);
    }

    public void GetUsernameAndSetMessage(IConnectionInfo connectionInfo)
    {
        Username = connectionInfo.Username;
        WelcomeMessage = $"{Username}, Добро пожаловать в информационную систему деканата!";
    }
}