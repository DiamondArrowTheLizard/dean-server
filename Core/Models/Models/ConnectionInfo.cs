using Interfaces.Models;
using Npgsql;

namespace Models.Models;

public class ConnectionInfo(string host, string database) : IConnectionInfo
{
    public string Host { get; set; } = host;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = database;

    public NpgsqlConnection? Connection { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Host) &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(Database);
    }

}