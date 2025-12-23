using System;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Shared;

namespace GUI.ViewModels.RoleMenus.ScientificSecretary;

public partial class ScientificSecretaryRoleViewModel : MenuViewModel
{
    public event Action? OnTeacherIndividualPlanOpen;
    public event Action? OnStudyGroupTeacherIndividualPlanOpen;
    public event Action? OnTeacherIndividualPlanKnowledgeCheckTypeOpen;
    public event Action? OnTeacherIndividualPlanDisciplineOpen;
    
    public override void SetupMenu(MainWindowViewModel view)
    {
        base.SetupMenu(view);
        OnTeacherIndividualPlanOpen += view.OpenTeacherIndividualPlan;
        OnStudyGroupTeacherIndividualPlanOpen += view.OpenStudyGroupTeacherIndividualPlan;
        OnTeacherIndividualPlanKnowledgeCheckTypeOpen += view.OpenTeacherIndividualPlanKnowledgeCheckType;
        OnTeacherIndividualPlanDisciplineOpen += view.OpenTeacherIndividualPlanDiscipline;
    }
    
    [RelayCommand]
    public void OpenTeacherIndividualPlan() => OnTeacherIndividualPlanOpen?.Invoke();
    
    [RelayCommand]
    public void OpenStudyGroupTeacherIndividualPlan() => OnStudyGroupTeacherIndividualPlanOpen?.Invoke();
    
    [RelayCommand]
    public void OpenTeacherIndividualPlanKnowledgeCheckType() => OnTeacherIndividualPlanKnowledgeCheckTypeOpen?.Invoke();
    
    [RelayCommand]
    public void OpenTeacherIndividualPlanDiscipline() => OnTeacherIndividualPlanDisciplineOpen?.Invoke();
}