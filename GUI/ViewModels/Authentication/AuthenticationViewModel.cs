
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfaces.Handlers.Authentication;
using Interfaces.Models;

namespace GUI.ViewModels.Authentication;

public partial class AuthenticationViewModel(
    IAuthenticationHandler authenticationHandler,
    IConnection connection
    ) : ViewModelBase
{
    private readonly IAuthenticationHandler _authenticationHandler = authenticationHandler;
    private readonly IConnection _connection = connection;

    [ObservableProperty]
    private string _login = ""; 

    [ObservableProperty]
    private string _password = "";

    public event Action<AuthenticationViewModel>? OnButtonClicked;

    [RelayCommand]
    public void HandleClick()
    {
        OnButtonClicked?.Invoke(this);

        Console.WriteLine("Attempted Authentication");
        _connection.Host = "localhost";
        _connection.Username = Login;
        _connection.Password = Password;
        _connection.Database = "DeanServer";

        _authenticationHandler.HandleAuthentication(_connection);
    }

}