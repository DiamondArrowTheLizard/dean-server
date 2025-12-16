
using Interfaces.Handlers.Authentication;

namespace Models.Handlers.Authentication;

public class ChangePassowrdHandler : IChangePassowrdHandler
{
    public void HandlePasswordChange(string login, string oldPassword, string newPassword, string newPasswordConfirm)
    {
        // TODO
    }
}