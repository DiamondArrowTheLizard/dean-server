using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
using GUI.ViewModels.Shared;
using Interfaces.Models;

namespace GUI.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IConnectionInfo _connectionInfo;

    [ObservableProperty]
    private AuthenticationViewModel _authenticationViewModel;

    [ObservableProperty]
    private ChangePasswordViewModel _changePasswordViewModel;

    [ObservableProperty]
    private TerminalWindowViewModel _terminalWindowViewModel;

    [ObservableProperty]
    private WelcomeScreenViewModel _welcomeScreenViewModel;

    [ObservableProperty]
    private ViewModelBase _currentView;


    public MainWindowViewModel(
    IConnectionInfo connectionInfo,
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel,
    TerminalWindowViewModel terminalWindowViewModel,
    WelcomeScreenViewModel welcomeScreenViewModel
    )
    {
        _connectionInfo = connectionInfo;
        
        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;
        
        TerminalWindowViewModel = terminalWindowViewModel;
        WelcomeScreenViewModel = welcomeScreenViewModel;

        CurrentView = AuthenticationViewModel;

        AuthenticationViewModel.OnButtonClicked += OnAuthentication;
    }

    public void ChangeView(ViewModelBase newView)
    {
        CurrentView = newView;
    }

    [RelayCommand]
    public void ChangePassword()
    {
        ChangeView(ChangePasswordViewModel);
    }

    [RelayCommand]
    public void GoBackToLogin() => ChangeView(AuthenticationViewModel);

    [RelayCommand]
    public void OnAuthentication(AuthenticationViewModel authenticationViewModel) 
    {
        WelcomeScreenViewModel.GetUsernameAndSetMessage(_connectionInfo);
        ChangeView(WelcomeScreenViewModel);
    } 
}
