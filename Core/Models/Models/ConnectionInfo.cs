using Interfaces.Models;

namespace Models.Models;

public class ConnectionInfo : IConnectionInfo
{
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Host) &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(Database);
    }
}