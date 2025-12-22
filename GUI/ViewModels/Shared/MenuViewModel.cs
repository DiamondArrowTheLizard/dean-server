using System;
using System.Runtime.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels.Shared;

public partial class MenuViewModel : ViewModelBase
{
    public event Action? OnMenuOpen;
    public event Action? OnTerminalOpen;
    public event Action? OnCityOpen;
    public event Action? OnStreetOpen;
    public event Action? OnSettingsOpen;
    public event Action? OnAboutOpen;

    public virtual void SetupMenu(MainWindowViewModel view)
    {
        OnMenuOpen += view.OpenMenu;
        OnTerminalOpen += view.OpenTerminal;
        OnCityOpen += view.OpenCity;
        OnStreetOpen += view.OpenStreet;
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
    public void OpenSettings()
    {
        OnSettingsOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenAbout()
    {
        OnAboutOpen?.Invoke();
    }
}