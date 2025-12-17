using Interfaces.Handlers.Authentication;
using Interfaces.Builders;
using Npgsql;
using Interfaces.Models;
using Models.Builders;

namespace Models.Handlers.Authentication;

public class ChangePasswordHandler(
    IConnectionInfo connectionInfo,
    IDatabaseConnectionString databaseConnectionString
) : IChangePasswordHandler
{
    private readonly IConnectionInfo _connectionInfo = connectionInfo;
    private readonly IDatabaseConnectionString _databaseConnectionString = databaseConnectionString;

    public bool HandlePasswordChange(string login, string oldPassword, string newPassword, string newPasswordConfirm)
    {
        Console.WriteLine($"ChangePassword Contents:\nLogin: {login}\nOld Password:{oldPassword}\nNew Password:{newPassword}\nNew Password Confirm:{newPasswordConfirm}");

        _connectionInfo.Host = "localhost";
        _connectionInfo.Username = login;
        _connectionInfo.Password = oldPassword;
        _connectionInfo.Database = "DeanServer";

        if (newPassword != newPasswordConfirm)
        {
            throw new ArgumentException("Новые пароли не совпадают.");
        }

        var builder = new ConnectionStringBuilder(_connectionInfo);
        builder.Build();
        _databaseConnectionString.ConnectionString = builder.GetConnectionString();
        string connectionString = _databaseConnectionString.ConnectionString;

        if (string.IsNullOrWhiteSpace(_connectionInfo.Host) ||
            string.IsNullOrWhiteSpace(_connectionInfo.Username) ||
            string.IsNullOrWhiteSpace(_connectionInfo.Password) ||
            string.IsNullOrWhiteSpace(_connectionInfo.Database))
        {
            Console.WriteLine("Ошибка: обязательные поля (имя пользователя, пароль) не заполнены");
            return false;
        }

        _connectionInfo.Connection = new NpgsqlConnection(connectionString);
        _connectionInfo.Connection.Open();

        string safeLogin = login.Replace("\"", "\"\"");
        string safePassword = newPassword.Replace("'", "''");

        string sql = $"ALTER USER \"{safeLogin}\" WITH PASSWORD '{safePassword}';";
        using var cmd = new NpgsqlCommand(sql, _connectionInfo.Connection);
        cmd.ExecuteNonQuery();

        _connectionInfo.Connection.Close();
        return true;
    }
}