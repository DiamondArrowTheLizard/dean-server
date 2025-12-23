using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
using GUI.ViewModels.RoleMenus.Admin;
using GUI.ViewModels.RoleMenus.Dean;
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
    private MenuViewModel _menuViewModel;

    [ObservableProperty]
    DeanRoleViewModel _deanRoleViewModel;

    [ObservableProperty]
    AdminRoleViewModel _adminRoleViewModel;

    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private MenuViewModel? _currentMenu;


    public MainWindowViewModel(
    IConnectionInfo connectionInfo,
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel,
    TerminalWindowViewModel terminalWindowViewModel,
    WelcomeScreenViewModel welcomeScreenViewModel,
    MenuViewModel menuViewModel,
    DeanRoleViewModel deanRoleViewModel,
    AdminRoleViewModel adminRoleViewModel
    )
    {
        _connectionInfo = connectionInfo;

        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;

        TerminalWindowViewModel = terminalWindowViewModel;
        
        WelcomeScreenViewModel = welcomeScreenViewModel;
        MenuViewModel = menuViewModel;

        DeanRoleViewModel = deanRoleViewModel;
        AdminRoleViewModel = adminRoleViewModel;

        CurrentView = AuthenticationViewModel;
        CurrentMenu = null;

        AuthenticationViewModel.OnButtonClicked += OnAuthentication;
        
    }

    public void ChangeView(ViewModelBase newView)
    {
        CurrentView = newView;
    }

    public void ChangeMenuView(MenuViewModel newView)
    {
        CurrentMenu = newView;
        CurrentMenu.SetupMenu(this);
    }


    [RelayCommand]
    public void OnAuthentication(AuthenticationViewModel authenticationViewModel)
    {
        switch(_connectionInfo.UserRole)
        {
            case "dean":
                ChangeMenuView(DeanRoleViewModel);
                break;

            case "admin":
                ChangeMenuView(AdminRoleViewModel);
                break;

            default:
                ChangeMenuView(MenuViewModel);
                break;
        }

        ChangeToWelcomeView(WelcomeScreenViewModel);
    }

    private void ChangeToWelcomeView(WelcomeScreenViewModel view)
    {
        view.GetUsernameAndSetMessage(_connectionInfo);
        ChangeView(view);
    }

    [RelayCommand]
    public void OpenLogin() {
        CurrentMenu = null;
        _connectionInfo.Username = string.Empty;
        _connectionInfo.Password = string.Empty;
        _connectionInfo.UserRole = string.Empty;
        ChangeView(AuthenticationViewModel);
    } 

    [RelayCommand]
    public void OpenChangePassword() => ChangeView(ChangePasswordViewModel);

    [RelayCommand]
    public void OpenMenu() => ChangeView(WelcomeScreenViewModel);

    [RelayCommand]
    public void OpenTerminal() => ChangeView(TerminalWindowViewModel);

    // TODO

    [RelayCommand]
    public void OpenCity() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenStreet() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenDepartment() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenDiscipline() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenClassroom() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenUsers() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenAudit() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenManual() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenSettings() => ChangeView(TerminalWindowViewModel);
}
