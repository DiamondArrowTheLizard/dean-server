using Interfaces.Models;

namespace Models.Models;

public class DatabaseConnectionString : IDatabaseConnectionString
{
    public string ConnectionString { get; set; } = string.Empty;
}