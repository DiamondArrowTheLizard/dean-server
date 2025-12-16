
using Interfaces.Handlers.Authentication;
using System;

namespace GUI.ViewModels.Authentication;

public partial class ChangePasswordButtonViewModel(ChangePasswordViewModel changePasswordViewModel,
 AuthenticationViewModel authenticationViewModel,
 IChangePassowrdHandler changePasswordHandler) : ViewModelBase
{
    private readonly AuthenticationViewModel _authenticationViewModel = authenticationViewModel;
    private readonly ChangePasswordViewModel _changePasswordViewModel = changePasswordViewModel;
    private readonly IChangePassowrdHandler _changePasswordHandler = changePasswordHandler;
    public event Action<ChangePasswordViewModel>? OnButtonClicked;

    public void HandleClick()
    {
        Console.WriteLine("Attempted Password Change");
    }
}