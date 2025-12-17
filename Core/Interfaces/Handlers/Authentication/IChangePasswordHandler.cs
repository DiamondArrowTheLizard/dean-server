
namespace Interfaces.Handlers.Authentication;

public interface IChangePasswordHandler
{
    public bool HandlePasswordChange(string login, string oldPassword, string newPassword, string newPasswordConfirm);
}