using CommunityToolkit.Mvvm.ComponentModel;
using GUI.ViewModels.Authentication;

namespace GUI.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private AuthenticationViewModel _authenticationViewModel;


    [ObservableProperty]
    private ChangePasswordViewModel _changePasswordViewModel;

    [ObservableProperty]
    private ChangePasswordButtonViewModel _changePasswordButtonViewModel;

    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel(
    AuthenticationViewModel authenticationViewModel,
    ChangePasswordViewModel changePasswordViewModel,
    ChangePasswordButtonViewModel changePasswordButtonViewModel
    )
    {
        AuthenticationViewModel = authenticationViewModel;
        ChangePasswordViewModel = changePasswordViewModel;
        ChangePasswordButtonViewModel = changePasswordButtonViewModel;

        CurrentView = AuthenticationViewModel;
    }

    public void ChangeView(ViewModelBase newView)
    {
        CurrentView = newView;
    }
}
