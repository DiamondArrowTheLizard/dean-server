
using System;
using Interfaces.Handlers.Authentication;

namespace GUI.ViewModels.Authentication;

public partial class AuthenticationButtonViewModel(AuthenticationViewModel authenticationViewModel, IAuthenticationHandler authenticationHandler) : ViewModelBase
{
    private readonly IAuthenticationHandler _authenticationHandler = authenticationHandler;

    private readonly AuthenticationViewModel _authenticationViewModel = authenticationViewModel;

    public event Action<AuthenticationButtonViewModel>? OnButtonClicked;

    public void HandleClick()
    {
        Console.WriteLine("Attempted Authentication");
        var login = _authenticationViewModel.Login;
        var password = _authenticationViewModel.Password;
        Console.WriteLine($"AuthVM contents:\nLogin: {login}\nPassword: {password}");
    }
}