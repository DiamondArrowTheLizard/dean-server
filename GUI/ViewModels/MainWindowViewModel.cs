using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
using GUI.ViewModels.RoleWindows.Dean;
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
    DeanRoleViewModel _deanRoleViewModel;

    [ObservableProperty]
    private ViewModelBase _currentView;


    public MainWindowViewModel(
    IConnectionInfo connectionInfo,
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel,
    TerminalWindowViewModel terminalWindowViewModel,
    WelcomeScreenViewModel welcomeScreenViewModel,
    DeanRoleViewModel deanRoleViewModel
    )
    {
        _connectionInfo = connectionInfo;

        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;

        TerminalWindowViewModel = terminalWindowViewModel;
        WelcomeScreenViewModel = welcomeScreenViewModel;

        DeanRoleViewModel = deanRoleViewModel;

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
    public void OpenTerminal() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OnAuthentication(AuthenticationViewModel authenticationViewModel)
    {
        switch (_connectionInfo.UserRole)
        {
            case "dean":
                ChangeRoleView(DeanRoleViewModel);
                break;

            default:
                ChangeRoleView(WelcomeScreenViewModel);
                break;
        }

    }

    private void ChangeRoleView(WelcomeScreenViewModel view)
    {
        view.GetUsernameAndSetMessage(_connectionInfo);
        view.OnOpenTerminal += OpenTerminal;
        ChangeView(view);

    }
}
