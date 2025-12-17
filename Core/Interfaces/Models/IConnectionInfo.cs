
namespace Interfaces.Models;

public interface IConnectionInfo
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }

    public bool IsValid();
}