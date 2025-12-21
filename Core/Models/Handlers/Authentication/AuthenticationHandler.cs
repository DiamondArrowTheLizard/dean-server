using Interfaces.Builders;
using Interfaces.Handlers.Authentication;
using Interfaces.Models;
using Models.Builders;
using Npgsql;

namespace Models.Handlers.Authentication;

public class AuthenticationHandler(IDatabaseConnectionString databaseConnectionString) : IAuthenticationHandler
{
    private readonly IDatabaseConnectionString _databaseConnectionString = databaseConnectionString;
    public bool HandleAuthentication(IConnectionInfo connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        // Console.WriteLine($"AuthVM contents:\nHost: {connection.Host}\nLogin: {connection.Username}\nPassword: {connection.Password}\nDatabase: {connection.Database}");

        try
        {
            
            if (string.IsNullOrWhiteSpace(connection.Host) ||
                string.IsNullOrWhiteSpace(connection.Username) ||
                string.IsNullOrWhiteSpace(connection.Password) ||
                string.IsNullOrWhiteSpace(connection.Database))
            {
                Console.WriteLine("Ошибка: обязательные поля (имя пользователя, пароль) не заполнены");
                return false;
            }

            
            var builder = new ConnectionStringBuilder(connection);
            builder.Build();
            _databaseConnectionString.ConnectionString = builder.GetConnectionString();
            string connectionString = _databaseConnectionString.ConnectionString;
            Console.WriteLine($"\n\nСтрока подключения:\n{connectionString}");


            connection.Connection = new NpgsqlConnection(connectionString);
            connection.Connection.Open();


            using (var cmd = new NpgsqlCommand("SELECT current_user, current_database()", connection.Connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Console.WriteLine($"Успешное подключение: пользователь {reader.GetString(0)}, база данных {reader.GetString(1)}");
                }
            }

            Console.WriteLine("Соединение с БД деканата установлено");
            return true;
        }
        catch (PostgresException ex) when (ex.SqlState == "28P01") 
        {
            Console.WriteLine($"Ошибка аутентификации: неверный пароль для пользователя '{connection.Username}'");
            return false;
        }
        catch (PostgresException ex)
        {
            Console.WriteLine($"Ошибка PostgreSQL [{ex.SqlState}]: {ex.Message}");
            return false;
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Ошибка Npgsql: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Общая ошибка: {ex.Message}");
            return false;
        }

        finally
        {
            Console.WriteLine("Закрываем подключение");
            connection.Connection?.Close();
        }

    }
}