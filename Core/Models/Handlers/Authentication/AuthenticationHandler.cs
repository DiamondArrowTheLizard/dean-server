
using Interfaces.Builders;
using Interfaces.Handlers.Authentication;
using Interfaces.Models;
using Npgsql;

namespace Models.Handlers.Authentication;

public class AuthenticationHandler(
    IDatabaseConnectionString databaseConnectionString,
    IConnectionStringBuilder connectionStringBuilder
) : IAuthenticationHandler
{
    private readonly IConnectionStringBuilder _connectionStringBuilder = connectionStringBuilder;
    private IDatabaseConnectionString _databaseConnectionString = databaseConnectionString;
    public void HandleAuthentication(IConnectionInfo connection)
    {
        Console.WriteLine($"AuthVM contents:\nHost: {connection.Host}\nLogin: {connection.Username}\nPassword: {connection.Password}\nDatabase: {connection.Database}");
        _connectionStringBuilder.AddHost();
        _connectionStringBuilder.AddUsername();
        _connectionStringBuilder.AddPassword();
        _connectionStringBuilder.AddDatabase();

        _databaseConnectionString = _connectionStringBuilder.GetResult();

        Console.WriteLine($"\n\nDatabaseConnectionString contents:");
        Console.WriteLine($"{_databaseConnectionString.ConnectionString}");

        using(var deanSystemConnection = new NpgsqlConnection(_databaseConnectionString.ConnectionString))
        {
            try {
                deanSystemConnection.Open();
                Console.WriteLine("Соединение с БД деканата установлено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка с соединением к серверу: {ex.Message}");
            }

        }
    }
}