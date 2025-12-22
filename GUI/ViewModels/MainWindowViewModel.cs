using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
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
    DeanRoleViewModel deanRoleViewModel
    )
    {
        _connectionInfo = connectionInfo;

        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;

        TerminalWindowViewModel = terminalWindowViewModel;
        
        WelcomeScreenViewModel = welcomeScreenViewModel;
        MenuViewModel = menuViewModel;

        DeanRoleViewModel = deanRoleViewModel;

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
        ChangeToWelcomeView(WelcomeScreenViewModel);
        switch(_connectionInfo.UserRole)
        {
            case "dean":
                ChangeMenuView(DeanRoleViewModel);
                break;

            default:
                ChangeMenuView(MenuViewModel);
                break;
        }
    }

    private void ChangeToWelcomeView(WelcomeScreenViewModel view)
    {
        view.GetUsernameAndSetMessage(_connectionInfo);
        ChangeView(view);
    }

    [RelayCommand]
    public void ChangePassword() => ChangeView(ChangePasswordViewModel);

    [RelayCommand]
    public void GoBackToLogin() => ChangeView(AuthenticationViewModel);

    [RelayCommand]
    public void OpenMenu() => ChangeView(WelcomeScreenViewModel);

    [RelayCommand]
    public void OpenTerminal() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenCity() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenStreet() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenFaculty() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenAcademicTitle() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenAcademicDegree() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenPosition() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenDiscipline() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenClassroom() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenKnowledgeCheckType() => ChangeView(TerminalWindowViewModel);

}
