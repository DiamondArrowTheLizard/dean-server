
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfaces.Handlers.Authentication;

namespace GUI.ViewModels.Authentication;

public partial class AuthenticationViewModel(IAuthenticationHandler authenticationHandler) : ViewModelBase
{
    private readonly IAuthenticationHandler _authenticationHandler = authenticationHandler;

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


        _authenticationHandler.HandleAuthentication(Login, Password);
    }

}