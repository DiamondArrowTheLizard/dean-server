
namespace Interfaces.Handlers.Authentication;

public interface IChangePassowrdHandler
{
    public void HandlePasswordChange(string login, string oldPassword, string newPassword, string newPasswordConfirm);
}