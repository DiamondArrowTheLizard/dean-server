
using CommunityToolkit.Mvvm.ComponentModel;

namespace GUI.ViewModels.Authentication;

public partial class ChangePasswordViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _oldPassword = ""; 

    [ObservableProperty]
    private string _newPassword = "";

    [ObservableProperty]
    private string _newPasswordConfirm = "";
    
}