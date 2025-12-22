using System;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Shared;

namespace GUI.ViewModels.RoleMenus.Admin;

public partial class AdminRoleViewModel : MenuViewModel
{
    public event Action? OnUsersOpen;
    public event Action? OnAuditOpen;

    public override void SetupMenu(MainWindowViewModel view)
    {
        base.SetupMenu(view);
        OnUsersOpen += view.OpenUsers;
        OnAuditOpen += view.OpenAudit;
    }

    [RelayCommand]
    public void OpenUsers() => OnUsersOpen?.Invoke();

    [RelayCommand]
    public void OpenAudit() => OnAuditOpen?.Invoke();
}