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

            GetUserRole(connection);

            Console.WriteLine($"Роль пользователя: {connection.UserRole}");
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
            Console.WriteLine($"AuthVM contents:\nHost: {connection.Host}\nLogin: {connection.Username}\nPassword: {connection.Password}\nDatabase: {connection.Database}\nRole: {connection.UserRole}");
            connection.Connection?.Close();
        }
    }

    private void GetUserRole(IConnectionInfo connection)
    {
        if (connection.Connection == null || connection.Connection.State != System.Data.ConnectionState.Open)
            return;

        try
        {
            string roleQuery = @"
                -- Получаем все роли, в которых состоит текущий пользователь
                SELECT r.rolname 
                FROM pg_roles r 
                JOIN pg_auth_members m ON (m.roleid = r.oid) 
                JOIN pg_roles u ON (u.oid = m.member) 
                WHERE u.rolname = current_user 
                AND r.rolname IN ('admin', 'dean', 'methodist', 'head_of_department', 'scientific_secretary', 'teacher')
                LIMIT 1;
            ";

            using (var cmd = new NpgsqlCommand(roleQuery, connection.Connection))
            {
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    connection.UserRole = result.ToString()!;
                    return;
                }
            }

            string currentUserQuery = "SELECT current_user;";
            using (var cmd = new NpgsqlCommand(currentUserQuery, connection.Connection))
            {
                var currentUser = cmd.ExecuteScalar()?.ToString();
                
                
                if (currentUser != null && currentUser.ToLower() == connection.Username.ToLower())
                {
                    
                    CheckUserPrivileges(connection, currentUser);
                }
            }

            
            if (string.IsNullOrEmpty(connection.UserRole))
            {
                connection.UserRole = "unknown";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении роли: {ex.Message}");
            connection.UserRole = "error";
        }
    }

    private void CheckUserPrivileges(IConnectionInfo connection, string username)
    {
        try
        {
            string privilegeQuery = @"
                SELECT 
                    CASE 
                        WHEN has_table_privilege(current_user, 'faculty', 'INSERT') THEN 'admin'
                        WHEN has_table_privilege(current_user, 'student', 'SELECT') AND 
                             has_table_privilege(current_user, 'facultyorder', 'INSERT') THEN 'methodist'
                        WHEN has_table_privilege(current_user, 'teacherindividualplan', 'INSERT') THEN 'scientific_secretary'
                        WHEN has_table_privilege(current_user, 'performance', 'INSERT') THEN 'teacher'
                        WHEN has_table_privilege(current_user, 'faculty', 'SELECT') THEN 'dean'
                        ELSE 'unknown'
                    END as user_role;
            ";

            using (var cmd = new NpgsqlCommand(privilegeQuery, connection.Connection))
            {
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    connection.UserRole = result.ToString()!;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при проверке привилегий: {ex.Message}");
        }
    }
}