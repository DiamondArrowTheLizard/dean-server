using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;

namespace GUI.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private AuthenticationViewModel _authenticationViewModel;


    [ObservableProperty]
    private ChangePasswordViewModel _changePasswordViewModel;


    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel(
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel)
    {
        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;

        CurrentView = AuthenticationViewModel;
    }

    public void ChangeView(ViewModelBase newView)
    {
        CurrentView = newView;
    }

    [RelayCommand]
    public void ChangePassword()
    {
        ChangeView(ChangePasswordViewModel);
    }

    [RelayCommand]
    public void GoBackToLogin() => ChangeView(AuthenticationViewModel);
}
