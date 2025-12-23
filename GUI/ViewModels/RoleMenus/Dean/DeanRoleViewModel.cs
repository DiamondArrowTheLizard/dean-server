using System;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Shared;
using Interfaces.Models;

namespace GUI.ViewModels.RoleMenus.Dean;

public partial class DeanRoleViewModel : MenuViewModel
{
    public event Action? OnDepartmentOpen;
    public event Action? OnClassroomOpen;

    public override void SetupMenu(MainWindowViewModel view)
    {
        base.SetupMenu(view);
        OnDepartmentOpen += view.OpenDepartment;
        OnClassroomOpen += view.OpenClassroom;
    }

    [RelayCommand]
    public void OpenDepartment()
    {
        OnDepartmentOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenClassroom()
    {
        OnClassroomOpen?.Invoke();
    }
}