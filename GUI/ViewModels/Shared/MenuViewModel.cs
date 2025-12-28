using System;
using System.Runtime.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels.Shared;

public partial class MenuViewModel : ViewModelBase
{
    public event Action? OnLogoutOpen;
    public event Action? OnMenuOpen;
    public event Action? OnTerminalOpen;
    public event Action? OnCityOpen;
    public event Action? OnStreetOpen;
    public event Action? OnAboutOpen;
    public event Action? OnManualOpen;
    public event Action? OnSettingsOpen;
    public event Action? OnChangePasswordOpen;
    public event Action? OnChartsOpen;

    public virtual void SetupMenu(MainWindowViewModel view)
    {
        OnLogoutOpen += view.OpenLogin;
        OnMenuOpen += view.OpenMenu;
        OnTerminalOpen += view.OpenTerminal;
        OnCityOpen += view.OpenCity;
        OnStreetOpen += view.OpenStreet;
        OnManualOpen += view.OpenManual;
        OnSettingsOpen += view.OpenSettings;
        OnChangePasswordOpen += view.OpenChangePassword;
        OnChartsOpen += view.OpenCharts;
    }

    [RelayCommand]
    public void OpenLogout()
    {
        OnLogoutOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenMenu()
    {
        OnMenuOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenTerminal()
    {
        OnTerminalOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenCity()
    {
        OnCityOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenStreet()
    {
        OnStreetOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenAbout()
    {
        OnAboutOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenManual()
    {
        OnManualOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenSettings()
    {
        OnSettingsOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenChangePassword()
    {
        OnChangePasswordOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenCharts()
    {
        OnChartsOpen?.Invoke();
    }

}