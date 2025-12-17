using Interfaces.Builders;
using Interfaces.Models;
using Npgsql;

namespace Models.Builders;

public class ConnectionStringBuilder(IConnectionInfo connection) : IConnectionStringBuilder
{
    private readonly IConnectionInfo _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    private readonly NpgsqlConnectionStringBuilder _npgsqlBuilder = [];

    public void Build()
    {
        _npgsqlBuilder.Host = _connection.Host;
        _npgsqlBuilder.Username = _connection.Username;
        _npgsqlBuilder.Password = _connection.Password;
        _npgsqlBuilder.Database = _connection.Database;

        
        _npgsqlBuilder.SslMode = SslMode.Prefer; 
        _npgsqlBuilder.Timeout = 15; 
        _npgsqlBuilder.CommandTimeout = 30;
    }

    public string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_npgsqlBuilder.Host) ||
            string.IsNullOrWhiteSpace(_npgsqlBuilder.Username))
        {
            throw new InvalidOperationException("Connection string is not properly built. Call Build() first.");
        }

        return _npgsqlBuilder.ConnectionString;
    }

    public bool ValidateConnection()
    {
        try
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                return connection.State == System.Data.ConnectionState.Open;
            }
        }
        catch
        {
            return false;
        }
    }
}