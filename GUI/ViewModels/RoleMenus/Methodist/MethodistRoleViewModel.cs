using System;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Shared;

namespace GUI.ViewModels.RoleMenus.Methodist;

public partial class MethodistRoleViewModel : MenuViewModel
{
    public event Action? OnStudyGroupOpen;
    public event Action? OnStudentOpen;
    public event Action? OnFacultyOrderOpen;
    
    public override void SetupMenu(MainWindowViewModel view)
    {
        base.SetupMenu(view);
        OnStudyGroupOpen += view.OpenStudyGroup;
        OnStudentOpen += view.OpenStudent;
        OnFacultyOrderOpen += view.OpenFacultyOrder;
    }
    
    [RelayCommand]
    public void OpenStudyGroup() => OnStudyGroupOpen?.Invoke();
    
    [RelayCommand]
    public void OpenStudent() => OnStudentOpen?.Invoke();
    
    [RelayCommand]
    public void OpenFacultyOrder() => OnFacultyOrderOpen?.Invoke();
}