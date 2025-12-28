namespace Core.Interfaces.Services;

public interface IQueryService
{
    Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query) where T : class, new();
    Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<IEnumerable<Dictionary<string, object?>>> ExecuteQueryRawAsync(string query);
    Task<T?> ExecuteScalarAsync<T>(string query, Dictionary<string, object>? parameters = null);
    string GetQuery(string queryName);
}