using Npgsql;
using Core.Interfaces.Services;
using Interfaces.Models;
using Models.Builders;

namespace Core.Models.Services;

public class QueryService : IQueryService
{
    private readonly IConnectionInfo _connectionInfo;
    private readonly IDatabaseConnectionString _connectionString;
    
    public QueryService(IConnectionInfo connectionInfo, IDatabaseConnectionString connectionString)
    {
        _connectionInfo = connectionInfo;
        _connectionString = connectionString;
    }
    
    public string GetQuery(string queryName)
    {
        var queries = new Dictionary<string, string>
        {
            ["GetAllDepartments"] = @"SELECT 
                d.id,
                d.department_name,
                f.faculty_name,
                d.id_faculty
            FROM Department d
            JOIN Faculty f ON d.id_faculty = f.id
            WHERE f.id = 1  -- Факультет Информатики и Вычислительной техники
            ORDER BY d.department_name;",
            
            ["GetAllFaculties"] = @"SELECT id, faculty_name FROM Faculty ORDER BY faculty_name;",
            
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
    
    public string ExecuteQuery(string query)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString.ConnectionString))
            {
                connection.Open();
                
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new System.Text.StringBuilder();
                        
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            result.Append(reader.GetName(i));
                            if (i < reader.FieldCount - 1) result.Append(" | ");
                        }
                        result.AppendLine();
                        result.AppendLine(new string('-', 50));
                        
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Append(reader[i]?.ToString() ?? "NULL");
                                if (i < reader.FieldCount - 1) result.Append(" | ");
                            }
                            result.AppendLine();
                        }
                        
                        return result.ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return $"Ошибка выполнения запроса: {ex.Message}";
        }
    }
}