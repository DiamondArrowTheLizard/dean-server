using CommunityToolkit.Mvvm.ComponentModel;

namespace GUI.ViewModels;

public partial class ProgramInfoViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _programName = "Информационная система Деканат ВУЗа";

    [ObservableProperty]
    private string _authorName = "Дементьев Артём Андреевич";

    [ObservableProperty]
    private string _studentGroup = "АВТ-313";

    [ObservableProperty]
    private string _license = "GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007";

    [ObservableProperty]
    private string _version = "Версия 1.0.0";
}