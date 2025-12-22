using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels.Shared;

public partial class MenuViewModel : ViewModelBase
{
    public event Action? OnMenuOpen;
    public event Action? OnTerminalOpen;
    public event Action? OnCityOpen;
    public event Action? OnStreetOpen;
    public event Action? OnFacultyOpen;
    public event Action? OnAcademicTitleOpen;
    public event Action? OnAcademicDegreeOpen;
    public event Action? OnDisciplineOpen;
    public event Action? OnPositionOpen;
    public event Action? OnClassroomOpen;

    public void SetupMenu(MainWindowViewModel view)
    {
        ArgumentNullException.ThrowIfNull(view);
        OnMenuOpen += view.OpenMenu;
        OnTerminalOpen += view.OpenTerminal;
        OnCityOpen += view.OpenCity;
        OnStreetOpen += view.OpenStreet;
        OnFacultyOpen += view.OpenFaculty;
        OnAcademicTitleOpen += view.OpenAcademicTitle;
        OnAcademicDegreeOpen += view.OpenAcademicDegree;
        OnDisciplineOpen += view.OpenDiscipline;
        OnPositionOpen += view.OpenPosition;
        OnClassroomOpen += view.OpenClassroom;
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
    public void OpenFaculty()
    {
        OnFacultyOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenAcademicTitle()
    {
        OnAcademicTitleOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenAcademicDegree()
    {
        OnAcademicDegreeOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenDiscipline()
    {
        OnDisciplineOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenPosition()
    {
        OnPositionOpen?.Invoke();
    }

    [RelayCommand]
    public void OpenClassroom()
    {
        OnClassroomOpen?.Invoke();
    }

}