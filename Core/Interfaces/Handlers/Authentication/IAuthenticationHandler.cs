
namespace Interfaces.Handlers.Authentication;

public interface IAuthenticationHandler
{
    public void HandleAuthentication(string login, string password);
}