using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels.Authentication;
using GUI.ViewModels.Shared;

namespace GUI.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private AuthenticationViewModel _authenticationViewModel;

    [ObservableProperty]
    private ChangePasswordViewModel _changePasswordViewModel;

    [ObservableProperty]
    private TerminalWindowViewModel _terminalWindowViewModel;

    [ObservableProperty]
    private ViewModelBase _currentView;


    public MainWindowViewModel(
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel,
    TerminalWindowViewModel terminalWindowViewModel
    )
    {
        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;
        
        TerminalWindowViewModel = terminalWindowViewModel;

        CurrentView = AuthenticationViewModel;

        AuthenticationViewModel.OnButtonClicked += OnAuthentication;
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

    [RelayCommand]
    public void OnAuthentication(AuthenticationViewModel authenticationViewModel) => ChangeView(TerminalWindowViewModel);
}
