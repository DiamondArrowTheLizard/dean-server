using System;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Shared;

namespace GUI.ViewModels.RoleMenus.HeadOfDepartment;

public partial class HeadOfDepartmentRoleViewModel : MenuViewModel
{
    public event Action? OnDepartmentOpen;
    public event Action? OnTeacherOpen;
    public event Action? OnDisciplineOpen;
    public event Action? OnTeacherIndividualPlanOpen;
    public event Action? OnStudyGroupOpen;
    public event Action? OnStudentOpen;
    public event Action? OnPerformanceOpen;
    public event Action? OnScheduleOpen;
    public event Action? OnCurriculumOpen;
    public event Action? OnQualificationWorkOpen;
    
    public override void SetupMenu(MainWindowViewModel view)
    {
        base.SetupMenu(view);
        OnDepartmentOpen += view.OpenDepartment;
        OnTeacherOpen += view.OpenTeacher;
        OnDisciplineOpen += view.OpenDiscipline;
        OnTeacherIndividualPlanOpen += view.OpenTeacherIndividualPlan;
        OnStudyGroupOpen += view.OpenStudyGroup;
        OnStudentOpen += view.OpenStudent;
        OnPerformanceOpen += view.OpenPerformance;
        OnScheduleOpen += view.OpenSchedule;
        OnCurriculumOpen += view.OpenCurriculum;
        OnQualificationWorkOpen += view.OpenQualificationWork;
    }
    
    [RelayCommand]
    public void OpenDepartment() => OnDepartmentOpen?.Invoke();
    
    [RelayCommand]
    public void OpenTeacher() => OnTeacherOpen?.Invoke();
    
    [RelayCommand]
    public void OpenDiscipline() => OnDisciplineOpen?.Invoke();
    
    [RelayCommand]
    public void OpenTeacherIndividualPlan() => OnTeacherIndividualPlanOpen?.Invoke();
    
    [RelayCommand]
    public void OpenStudyGroup() => OnStudyGroupOpen?.Invoke();
    
    [RelayCommand]
    public void OpenStudent() => OnStudentOpen?.Invoke();
    
    [RelayCommand]
    public void OpenPerformance() => OnPerformanceOpen?.Invoke();
    
    [RelayCommand]
    public void OpenSchedule() => OnScheduleOpen?.Invoke();
    
    [RelayCommand]
    public void OpenCurriculum() => OnCurriculumOpen?.Invoke();
    
    [RelayCommand]
    public void OpenQualificationWork() => OnQualificationWorkOpen?.Invoke();
}