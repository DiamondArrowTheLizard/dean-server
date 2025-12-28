using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GUI.ViewModels.Entities.Base;
using Interfaces.Services;
using Models.Models.Tables;

namespace GUI.ViewModels.Entities;

public partial class StreetViewModel(IQueryService queryService) : BaseCrudViewModel<StreetDisplay>(queryService)
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
        return _queryService.GetQuery("GetAllStreets");
    }

    protected override async Task InsertItemAsync(StreetDisplay item)
    {
        var query = _queryService.GetQuery("InsertStreet");
        var parameters = new Dictionary<string, object>
        {
            ["streetName"] = item.StreetName
        };

        var newId = await _queryService.ExecuteScalarAsync<int>(query, parameters);
        item.Id = newId;
    }

    protected override async Task UpdateItemAsync(StreetDisplay item)
    {
        var query = _queryService.GetQuery("UpdateStreet");
        var parameters = new Dictionary<string, object>
        {
            ["streetName"] = item.StreetName,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override async Task DeleteItemAsync(StreetDisplay item)
    {
        var query = _queryService.GetQuery("DeleteStreetWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override StreetDisplay CreateNewItem()
    {
        return new StreetDisplay
        {
            Id = -1,
            StreetName = "Новая улица"
        };
    }

    protected override async Task<bool> ConfirmDeleteAsync()
    {
        return await Task.FromResult(true);
    }

    protected override IEnumerable<StreetDisplay> FilterItems(string searchText)
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
                (d.StreetName != null && d.StreetName.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;
        
        var csv = new StringBuilder();
        
        csv.AppendLine("Id,StreetName");
        
        foreach (var item in itemsToExport)
        {
            var escapedStreetName = EscapeCsvField(item.StreetName ?? "");
            
            csv.AppendLine($"{item.Id},{escapedStreetName}");
        }
        
        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);
        
        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);
        
        var fileName = $"Streets_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class StreetDisplay : TableBase
{
    public string StreetName { get; set; } = string.Empty;

    public StreetDisplay() : base(0) { }
    public StreetDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return StreetName;
    }
}