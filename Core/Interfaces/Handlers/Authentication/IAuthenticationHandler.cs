
using Interfaces.Models;

namespace Interfaces.Handlers.Authentication;

public interface IAuthenticationHandler
{
    public bool HandleAuthentication(IConnectionInfo connection);
}