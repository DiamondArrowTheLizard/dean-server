using Interfaces.Handlers.Shared;
using Interfaces.Models;
using Npgsql;

namespace Models.Handlers.Shared;

public class TerminalQueryHandler(IConnectionInfo connectionInfo) : ITerminalQueryHandler
{
    private readonly IConnectionInfo _connectionInfo = connectionInfo;
    private readonly List<string> queryResult = [];
    private readonly List<List<string>> queryResultList = [];

    public bool HandleTerminalQuery(string queryString, out string outputString)
    {
        outputString = string.Empty;
        
        ArgumentNullException.ThrowIfNull(_connectionInfo.Connection);
        
        _connectionInfo.Connection.Open();
        using var command = new NpgsqlCommand($"{queryString}", _connectionInfo.Connection);
        using var reader = command.ExecuteReader();
        
        var allResults = new List<string>();
        while (reader.Read())
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var str = reader[i].ToString();
                ArgumentNullException.ThrowIfNull(str);
                queryResult.Add(str);
            }
            queryResultList.Add([.. queryResult]);
            allResults.Add(string.Join(" ", queryResult));
            queryResult.Clear();
        }
        
        outputString = string.Join("\n", allResults);
        _connectionInfo.Connection.Close();
        return true;
    }
}