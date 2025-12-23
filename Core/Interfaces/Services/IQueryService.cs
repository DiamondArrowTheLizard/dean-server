using System.Collections.Generic;

namespace Core.Interfaces.Services;

public interface IQueryService
{
    string GetQuery(string queryName);
    Dictionary<string, string> GetAvailableQueries();
    string ExecuteQuery(string query);
}