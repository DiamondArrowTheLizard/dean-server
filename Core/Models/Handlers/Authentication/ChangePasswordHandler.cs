using Interfaces.Handlers.Authentication;
using Interfaces.Builders;
using Npgsql;
using Interfaces.Models;
using Models.Builders;

namespace Models.Handlers.Authentication;

public class ChangePasswordHandler(
    IConnectionStringBuilder connectionStringBuilder,
    IAuthenticationHandler authenticationHandler,
    IConnectionInfo connectionInfo
) : IChangePasswordHandler
{
    private readonly IConnectionStringBuilder _connectionStringBuilder = connectionStringBuilder;
    private readonly IAuthenticationHandler _authenticationHandler = authenticationHandler;
    private readonly IConnectionInfo _connectionInfo = connectionInfo;

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
        string connectionString = builder.GetConnectionString();

        if (string.IsNullOrWhiteSpace(_connectionInfo.Host) ||
            string.IsNullOrWhiteSpace(_connectionInfo.Username) ||
            string.IsNullOrWhiteSpace(_connectionInfo.Password) ||
            string.IsNullOrWhiteSpace(_connectionInfo.Database))
        {
            Console.WriteLine("Ошибка: обязательные поля (имя пользователя, пароль) не заполнены");
            return false;
        }

        using var deanSystemConnection = new NpgsqlConnection(connectionString);
        deanSystemConnection.Open();

        string safeLogin = login.Replace("\"", "\"\"");
        string safePassword = newPassword.Replace("'", "''");

        string sql = $"ALTER USER \"{safeLogin}\" WITH PASSWORD '{safePassword}';";
        using var cmd = new NpgsqlCommand(sql, deanSystemConnection);
        cmd.ExecuteNonQuery();

        deanSystemConnection.Close();
        return true;
    }
}