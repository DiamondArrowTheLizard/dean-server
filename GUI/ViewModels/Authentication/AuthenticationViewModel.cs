
using CommunityToolkit.Mvvm.ComponentModel;

namespace GUI.ViewModels.Authentication;

public partial class AuthenticationViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _login = ""; 

    [ObservableProperty]
    private string _password = "";

}