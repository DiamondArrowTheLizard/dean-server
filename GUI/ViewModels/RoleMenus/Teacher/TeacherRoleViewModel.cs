using System;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Shared;

namespace GUI.ViewModels.RoleMenus.Teacher;

public partial class TeacherRoleViewModel : MenuViewModel
{
    public event Action? OnCurriculumOpen;
    public event Action? OnTeacherIndividualPlanOpen;
    public event Action? OnStudyGroupOpen;
    public event Action? OnStudentOpen;
    public event Action? OnDisciplineOpen;
    public event Action? OnScheduleOpen;
    public event Action? OnPerformanceOpen;
    
    public override void SetupMenu(MainWindowViewModel view)
    {
        base.SetupMenu(view);
        OnCurriculumOpen += view.OpenCurriculum;
        OnTeacherIndividualPlanOpen += view.OpenTeacherIndividualPlan;
        OnStudyGroupOpen += view.OpenStudyGroup;
        OnStudentOpen += view.OpenStudent;
        OnDisciplineOpen += view.OpenDiscipline;
        OnScheduleOpen += view.OpenSchedule;
        OnPerformanceOpen += view.OpenPerformance;
    }
    
    [RelayCommand]
    public void OpenCurriculum() => OnCurriculumOpen?.Invoke();
    
    [RelayCommand]
    public void OpenTeacherIndividualPlan() => OnTeacherIndividualPlanOpen?.Invoke();
    
    [RelayCommand]
    public void OpenStudyGroup() => OnStudyGroupOpen?.Invoke();
    
    [RelayCommand]
    public void OpenStudent() => OnStudentOpen?.Invoke();
    
    [RelayCommand]
    public void OpenDiscipline() => OnDisciplineOpen?.Invoke();
    
    [RelayCommand]
    public void OpenSchedule() => OnScheduleOpen?.Invoke();
    
    [RelayCommand]
    public void OpenPerformance() => OnPerformanceOpen?.Invoke();
}