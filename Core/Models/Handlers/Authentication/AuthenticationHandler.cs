
using Interfaces.Builders;
using Interfaces.Handlers.Authentication;
using Interfaces.Models;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Models.Builders;
using Models.Models;

namespace Models.Handlers.Authentication;

public class AuthenticationHandler(
    IDatabaseConnectionString databaseConnectionString,
    IConnectionStringBuilder connectionStringBuilder
) : IAuthenticationHandler
{
    private readonly IConnectionStringBuilder _connectionStringBuilder = connectionStringBuilder;
    private IDatabaseConnectionString _databaseConnectionString = databaseConnectionString;
    public void HandleAuthentication(IConnection connection)
    {
        Console.WriteLine($"AuthVM contents:\nHost: {connection.Host}\nLogin: {connection.Username}\nPassword: {connection.Password}\nDatabase: {connection.Database}");
        _connectionStringBuilder.AddHost();
        _connectionStringBuilder.AddUsername();
        _connectionStringBuilder.AddPassword();
        _connectionStringBuilder.AddDatabase();

        _databaseConnectionString = _connectionStringBuilder.GetResult();

        Console.WriteLine($"\n\nDatabaseConnectionString contents:");
        Console.WriteLine($"{_databaseConnectionString.ConnectionString}");
    }
}