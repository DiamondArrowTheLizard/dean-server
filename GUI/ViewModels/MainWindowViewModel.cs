using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
using GUI.ViewModels.Entities;
using GUI.ViewModels.RoleMenus.Admin;
using GUI.ViewModels.RoleMenus.Dean;
using GUI.ViewModels.RoleMenus.HeadOfDepartment;
using GUI.ViewModels.RoleMenus.Methodist;
using GUI.ViewModels.RoleMenus.ScientificSecretary;
using GUI.ViewModels.RoleMenus.Teacher;
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
    private MethodistRoleViewModel _methodistRoleViewModel;

    [ObservableProperty]
    private HeadOfDepartmentRoleViewModel _headOfDepartmentRoleViewModel;

    [ObservableProperty]
    private ScientificSecretaryRoleViewModel _scientificSecretaryRoleViewModel;

    [ObservableProperty]
    private TeacherRoleViewModel _teacherRoleViewModel;

    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private MenuViewModel? _currentMenu;

    [ObservableProperty]
    private DepartmentViewModel _departmentViewModel;

    public MainWindowViewModel(
    IConnectionInfo connectionInfo,
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel,
    TerminalWindowViewModel terminalWindowViewModel,
    WelcomeScreenViewModel welcomeScreenViewModel,
    MenuViewModel menuViewModel,
    DeanRoleViewModel deanRoleViewModel,
    AdminRoleViewModel adminRoleViewModel,
    MethodistRoleViewModel methodistRoleViewModel,
    HeadOfDepartmentRoleViewModel headOfDepartmentRoleViewModel,
    ScientificSecretaryRoleViewModel scientificSecretaryRoleViewModel,
    DepartmentViewModel departmentViewModel,
    TeacherRoleViewModel teacherRoleViewModel)
    {
        _connectionInfo = connectionInfo;

        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;

        TerminalWindowViewModel = terminalWindowViewModel;

        WelcomeScreenViewModel = welcomeScreenViewModel;
        MenuViewModel = menuViewModel;

        DeanRoleViewModel = deanRoleViewModel;
        AdminRoleViewModel = adminRoleViewModel;
        MethodistRoleViewModel = methodistRoleViewModel;
        HeadOfDepartmentRoleViewModel = headOfDepartmentRoleViewModel;
        ScientificSecretaryRoleViewModel = scientificSecretaryRoleViewModel;
        TeacherRoleViewModel = teacherRoleViewModel;

        CurrentView = AuthenticationViewModel;
        CurrentMenu = null;

        DepartmentViewModel = departmentViewModel;

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
        switch (_connectionInfo.UserRole.ToLower())
        {
            case "dean":
                ChangeMenuView(DeanRoleViewModel);
                break;
            case "admin":
                ChangeMenuView(AdminRoleViewModel);
                break;
            case "methodist":
                ChangeMenuView(MethodistRoleViewModel);
                break;
            case "head_of_department":
                ChangeMenuView(HeadOfDepartmentRoleViewModel);
                break;
            case "scientific_secretary":
                ChangeMenuView(ScientificSecretaryRoleViewModel);
                break;
            case "teacher":
                ChangeMenuView(TeacherRoleViewModel);
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
    public void OpenLogin()
    {
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
    public void OpenDepartment() => ChangeView(DepartmentViewModel);

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

    [RelayCommand]
    public void OpenStudyGroup() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenStudent() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenFacultyOrder() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacher() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacherIndividualPlan() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenPerformance() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenSchedule() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenCurriculum() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenQualificationWork() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenStudyGroupTeacherIndividualPlan() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacherIndividualPlanKnowledgeCheckType() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacherIndividualPlanDiscipline() => ChangeView(TerminalWindowViewModel);

}
