
using Interfaces.Handlers.Authentication;

namespace Models.Handlers.Authentication;

public class ChangePasswordHandler : IChangePassowrdHandler
{
    public void HandlePasswordChange(string login, string oldPassword, string newPassword, string newPasswordConfirm)
    {
        // TODO
        Console.WriteLine($"ChangePassword Contents:\nLogin: {login}\nOld Password:{oldPassword}\nNew Password:{newPassword}\nNew Password Confirm:{newPasswordConfirm}");
    }
}