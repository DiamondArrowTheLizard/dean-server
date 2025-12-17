
using Interfaces.Builders;
using Interfaces.Models;

namespace Models.Builders;

public class ConnectionStringBuilder(IConnectionInfo connection, IDatabaseConnectionString databaseConnectionString) : IConnectionStringBuilder
{
    private readonly IConnectionInfo _connection = connection;
    protected IDatabaseConnectionString? _databaseConnectionString = databaseConnectionString;
    public IDatabaseConnectionString DatabaseConnectionString
    {
        get
        {
            return _databaseConnectionString ?? throw new ArgumentNullException(nameof(_databaseConnectionString));
        }
    }

    public void AddHost()
    {
        ArgumentNullException.ThrowIfNull(_databaseConnectionString);

        string hostString = $"Host=\'{_connection.Host}\'";
        _databaseConnectionString.ConnectionString = hostString;
    }

    public void AddUsername()
    {
        ArgumentNullException.ThrowIfNull(_databaseConnectionString);
        ArgumentNullException.ThrowIfNullOrEmpty(_databaseConnectionString.ConnectionString);

        string usernameString = $"Username=\'{_connection.Username}\'";
        _databaseConnectionString.ConnectionString += $";{usernameString}";

    }

    public void AddPassword()
    {
        ArgumentNullException.ThrowIfNull(_databaseConnectionString);
        ArgumentNullException.ThrowIfNullOrEmpty(_databaseConnectionString.ConnectionString);

        string passwordString = $"Password=\'{_connection.Password}\'";
        _databaseConnectionString.ConnectionString += $";{passwordString}";
    }
    
    public void AddDatabase()
    {
        ArgumentNullException.ThrowIfNull(_databaseConnectionString);
        ArgumentNullException.ThrowIfNullOrEmpty(_databaseConnectionString.ConnectionString);

        string databaseString = $"Database=\'{_connection.Database}\'";
        _databaseConnectionString.ConnectionString += $";{databaseString}";
    }

    public IDatabaseConnectionString GetResult() => _databaseConnectionString ?? throw new ArgumentNullException(nameof(_databaseConnectionString));
}