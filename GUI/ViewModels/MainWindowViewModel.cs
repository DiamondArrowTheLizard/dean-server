﻿﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
using GUI.ViewModels.Charts;
using GUI.ViewModels.Entities;
using GUI.ViewModels.Help;
using GUI.ViewModels.RoleMenus.Admin;
using GUI.ViewModels.RoleMenus.Dean;
using GUI.ViewModels.RoleMenus.HeadOfDepartment;
using GUI.ViewModels.RoleMenus.Methodist;
using GUI.ViewModels.RoleMenus.ScientificSecretary;
using GUI.ViewModels.RoleMenus.Teacher;
using GUI.ViewModels.Services;
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

    [ObservableProperty]
    private ClassroomViewModel _classroomViewModel;

    [ObservableProperty]
    private StudentViewModel _studentViewModel;

    [ObservableProperty]
    private ScheduleViewModel _scheduleViewModel;

    [ObservableProperty]
    private PerformanceViewModel _performanceViewModel;

    [ObservableProperty]
    private QualificationWorkViewModel _qualificationWorkViewModel;

    [ObservableProperty]
    private CurriculumViewModel _curriculumViewModel;

    [ObservableProperty]
    private StudyGroupViewModel _studyGroupViewModel;

    [ObservableProperty]
    private TeacherIndividualPlanViewModel _teacherIndividualPlanViewModel;

    [ObservableProperty]
    private CityViewModel _cityViewModel;

    [ObservableProperty]
    private StreetViewModel _streetViewModel;

    [ObservableProperty]
    private ChartsViewModel _chartsViewModel;

    [ObservableProperty]
    private ManualViewModel _manualViewModel;

    [ObservableProperty]
    private ProgramInfoViewModel _programInfoViewModel;

    [ObservableProperty]
    private SettingsViewModel _settingsViewModel;

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
        ClassroomViewModel classroomViewModel,
        StudentViewModel studentViewModel,
        TeacherRoleViewModel teacherRoleViewModel,
        ScheduleViewModel scheduleViewModel,
        PerformanceViewModel performanceViewModel,
        QualificationWorkViewModel qualificationWorkViewModel,
        CurriculumViewModel curriculumViewModel,
        StudyGroupViewModel studyGroupViewModel,
        TeacherIndividualPlanViewModel teacherIndividualPlanViewModel,
        CityViewModel cityViewModel,
        StreetViewModel streetViewModel,
        ChartsViewModel chartsViewModel,
        ManualViewModel manualViewModel,
        SettingsViewModel settingsViewModel,
        ProgramInfoViewModel programInfoViewModel)  
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
        ClassroomViewModel = classroomViewModel;
        StudentViewModel = studentViewModel;
        ScheduleViewModel = scheduleViewModel;
        PerformanceViewModel = performanceViewModel;
        QualificationWorkViewModel = qualificationWorkViewModel;
        CurriculumViewModel = curriculumViewModel;
        StudyGroupViewModel = studyGroupViewModel;
        TeacherIndividualPlanViewModel = teacherIndividualPlanViewModel;
        CityViewModel = cityViewModel;
        StreetViewModel = streetViewModel;

        ChartsViewModel = chartsViewModel;
        ManualViewModel = manualViewModel;
        ProgramInfoViewModel = programInfoViewModel;
        SettingsViewModel = settingsViewModel;

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

    [RelayCommand]
    public void OpenCity() => ChangeView(CityViewModel);

    [RelayCommand]
    public void OpenStreet() => ChangeView(StreetViewModel);

    [RelayCommand]
    public void OpenDepartment() => ChangeView(DepartmentViewModel);

    [RelayCommand]
    public void OpenDiscipline() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenClassroom() => ChangeView(ClassroomViewModel);

    [RelayCommand]
    public void OpenUsers() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenAudit() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenManual() => ChangeView(ManualViewModel);

    [RelayCommand]
    public void OpenSettings() => ChangeView(SettingsViewModel);

    [RelayCommand]
    public void OpenStudyGroup() => ChangeView(StudyGroupViewModel);

    [RelayCommand]
    public void OpenStudent() => ChangeView(StudentViewModel);

    [RelayCommand]
    public void OpenFacultyOrder() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacher() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacherIndividualPlan() => ChangeView(TeacherIndividualPlanViewModel);

    [RelayCommand]
    public void OpenPerformance() => ChangeView(PerformanceViewModel);

    [RelayCommand]
    public void OpenSchedule() => ChangeView(ScheduleViewModel);

    [RelayCommand]
    public void OpenCurriculum() => ChangeView(CurriculumViewModel);

    [RelayCommand]
    public void OpenQualificationWork() => ChangeView(QualificationWorkViewModel);

    [RelayCommand]
    public void OpenStudyGroupTeacherIndividualPlan() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacherIndividualPlanKnowledgeCheckType() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenTeacherIndividualPlanDiscipline() => ChangeView(TerminalWindowViewModel);

    [RelayCommand]
    public void OpenCharts() => ChangeView(ChartsViewModel);

    [RelayCommand]
    public void OpenAbout() => ChangeView(ProgramInfoViewModel);

}