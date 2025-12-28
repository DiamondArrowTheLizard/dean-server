using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.ViewModels.Entities.Base;
using Interfaces.Services;
using Models.Models.Tables;

namespace GUI.ViewModels.Entities;

public partial class CityViewModel(IQueryService queryService) : BaseCrudViewModel<CityDisplay>(queryService)
{
    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await base.LoadDataAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public override async Task AddNewAsync()
    {
        await base.AddNewAsync();
    }

    protected override string GetSelectQuery()
    {
        return _queryService.GetQuery("GetAllCities");
    }

    protected override async Task InsertItemAsync(CityDisplay item)
    {
        var query = _queryService.GetQuery("InsertCity");
        var parameters = new Dictionary<string, object>
        {
            ["cityName"] = item.CityName
        };

        var newId = await _queryService.ExecuteScalarAsync<int>(query, parameters);
        item.Id = newId;
    }

    protected override async Task UpdateItemAsync(CityDisplay item)
    {
        var query = _queryService.GetQuery("UpdateCity");
        var parameters = new Dictionary<string, object>
        {
            ["cityName"] = item.CityName,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override async Task DeleteItemAsync(CityDisplay item)
    {
        var query = _queryService.GetQuery("DeleteCityWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override CityDisplay CreateNewItem()
    {
        return new CityDisplay
        {
            Id = -1,
            CityName = "Новый город"
        };
    }

    protected override async Task<bool> ConfirmDeleteAsync()
    {
        return await Task.FromResult(true);
    }

    protected override IEnumerable<CityDisplay> FilterItems(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Items;
        }

        if (int.TryParse(searchText, out int id))
        {
            return Items.Where(item => item.Id == id);
        }
        else
        {
            var searchLower = searchText.ToLowerInvariant();

            return Items.Where(d =>
                (d.CityName != null && d.CityName.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;
        
        var csv = new StringBuilder();
        
        csv.AppendLine("Id,CityName");
        
        foreach (var item in itemsToExport)
        {
            var escapedCityName = EscapeCsvField(item.CityName ?? "");
            
            csv.AppendLine($"{item.Id},{escapedCityName}");
        }
        
        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);
        
        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);
        
        var fileName = $"Cities_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var filePath = Path.Combine(folderPath, fileName);
        
        await File.WriteAllTextAsync(filePath, data, Encoding.UTF8);
    }

    private static string FindSolutionRoot(string currentDirectory)
    {
        var directory = new DirectoryInfo(currentDirectory);
        
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }
        
        return directory?.FullName ?? currentDirectory;
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            field = field.Replace("\"", "\"\"");
            return $"\"{field}\"";
        }
        return field;
    }
}

public class CityDisplay : TableBase
{
    public string CityName { get; set; } = string.Empty;

    public CityDisplay() : base(0) { }
    public CityDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return CityName;
    }
}