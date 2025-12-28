using System.Reflection;
using Core.Interfaces.Services;
using Interfaces.Models;
using Models.Builders;
using Npgsql;

namespace Models.Services;

public class QueryService(IDatabaseConnectionString connectionString) : IQueryService
{
    private readonly IDatabaseConnectionString _connectionString = connectionString;

    public string GetQuery(string queryName)
    {
        var queries = new Dictionary<string, string>
        {
            ["GetAllDepartments"] = @"SELECT 
                d.id,
                d.department_name AS DepartmentName,
                f.faculty_name AS FacultyName,
                d.id_faculty AS IdFaculty
            FROM Department d
            JOIN Faculty f ON d.id_faculty = f.id
            ORDER BY d.id;",

            ["GetAllFaculties"] = @"SELECT id, faculty_name AS FacultyName FROM Faculty ORDER BY faculty_name;",

            ["InsertDepartment"] = @"INSERT INTO Department (department_name, id_faculty) 
                VALUES (@departmentName, @idFaculty) RETURNING id;",

            ["UpdateDepartment"] = @"UPDATE Department 
                SET department_name = @departmentName, id_faculty = @idFaculty 
                WHERE id = @id;",

            ["DeleteDepartment"] = @"DELETE FROM Department WHERE id = @id;"
        };

        return queries.ContainsKey(queryName) ? queries[queryName] : string.Empty;
    }

    public Dictionary<string, string> GetAvailableQueries()
    {
        return new Dictionary<string, string>
        {
            ["GetAllDepartments"] = "Получить все кафедры",
            ["GetAllFaculties"] = "Получить все факультеты",
            ["InsertDepartment"] = "Добавить кафедру",
            ["UpdateDepartment"] = "Обновить кафедру",
            ["DeleteDepartment"] = "Удалить кафедру"
        };
    }

    private void CheckConnection()
    {
        Console.WriteLine($"Department connection: {_connectionString.ConnectionString}");
        if (string.IsNullOrEmpty(_connectionString.ConnectionString))
        {
            throw new InvalidOperationException("Строка подключения не установлена. Выполните аутентификацию.");
        }
    }

    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query) where T : class, new()
    {
        CheckConnection();
        
        var results = new List<T>();
        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var propName = reader.GetName(i);
                    var prop = typeof(T).GetProperty(propName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (prop != null && !reader.IsDBNull(i))
                    {
                        var value = reader.GetValue(i);

                        if (prop.PropertyType == typeof(string))
                            prop.SetValue(item, value.ToString());
                        else
                            prop.SetValue(item, Convert.ChangeType(value, prop.PropertyType));
                    }
                }
                results.Add(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
            throw;
        }

        return results;
    }

    public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        CheckConnection();
        
        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            return await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения команды: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Dictionary<string, object?>>> ExecuteQueryRawAsync(string query)
    {
        CheckConnection();
        
        var results = new List<Dictionary<string, object?>>();
        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }
                results.Add(row);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
            throw;
        }

        return results;
    }

    public async Task<T?> ExecuteScalarAsync<T>(string query, Dictionary<string, object>? parameters = null)
    {
        CheckConnection();
        
        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            var result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? default : (T)Convert.ChangeType(result, typeof(T));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения скалярного запроса: {ex.Message}");
            throw;
        }
    }
}