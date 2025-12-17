
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfaces.Handlers.Authentication;

namespace GUI.ViewModels.Authentication;

public partial class ChangePasswordViewModel(AuthenticationViewModel authenticationViewModel, IChangePasswordHandler changePasswordHandler): ViewModelBase
{
    private readonly AuthenticationViewModel _authenticationViewModel = authenticationViewModel;
    private readonly IChangePasswordHandler _changePasswordHandler = changePasswordHandler;

    [ObservableProperty]
    private string _oldPassword = ""; 

    [ObservableProperty]
    private string _newPassword = "";

    [ObservableProperty]
    private string _newPasswordConfirm = "";
    
    public event Action<ChangePasswordViewModel>? OnButtonClicked;

    [RelayCommand]
    public void HandleClick()
    { 
        OnButtonClicked?.Invoke(this);

        Console.WriteLine("Attempted Password Change");

        _changePasswordHandler.HandlePasswordChange(_authenticationViewModel.Login, OldPassword, NewPassword, NewPasswordConfirm);
    }
}