
using Interfaces.Models;

namespace Interfaces.Handlers.Authentication;

public interface IAuthenticationHandler
{
    public void HandleAuthentication(IConnection connection);
}