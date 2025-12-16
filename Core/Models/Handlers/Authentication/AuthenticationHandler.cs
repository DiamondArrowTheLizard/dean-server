
using Interfaces.Handlers.Authentication;

namespace Models.Handlers.Authentication;

public class AuthenticationHandler : IAuthenticationHandler
{
    public void HandleAuthentication(string login, string password)
    {
        // TODO
        Console.WriteLine($"AuthVM contents:\nLogin: {login}\nPassword: {password}\n");
    }
}