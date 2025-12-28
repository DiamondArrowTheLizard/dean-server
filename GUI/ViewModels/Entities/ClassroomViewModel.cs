using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Interfaces.Services;
using GUI.ViewModels.Entities.Base;
using Models.Models.Tables;

namespace GUI.ViewModels.Entities;

public partial class ClassroomViewModel(IQueryService queryService) : BaseCrudViewModel<ClassroomDisplay>(queryService)
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
        return _queryService.GetQuery("GetAllClassrooms");
    }

    protected override async Task InsertItemAsync(ClassroomDisplay item)
    {
        var query = _queryService.GetQuery("InsertClassroom");
        var parameters = new Dictionary<string, object>
        {
            ["classroomName"] = item.ClassroomName
        };

        var newId = await _queryService.ExecuteScalarAsync<int>(query, parameters);
        item.Id = newId;
    }

    protected override async Task UpdateItemAsync(ClassroomDisplay item)
    {
        var query = _queryService.GetQuery("UpdateClassroom");
        var parameters = new Dictionary<string, object>
        {
            ["classroomName"] = item.ClassroomName,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override async Task DeleteItemAsync(ClassroomDisplay item)
    {
        var query = _queryService.GetQuery("DeleteClassroomWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override ClassroomDisplay CreateNewItem()
    {
        return new ClassroomDisplay
        {
            Id = -1,
            ClassroomName = "Новая аудитория"
        };
    }

    protected override async Task<bool> ConfirmDeleteAsync()
    {
        return await Task.FromResult(true);
    }

    protected override IEnumerable<ClassroomDisplay> FilterItems(string searchText)
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
                (d.ClassroomName != null && d.ClassroomName.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;
        
        var csv = new StringBuilder();
        
        csv.AppendLine("Id,ClassroomName");
        
        foreach (var item in itemsToExport)
        {
            var escapedClassroomName = EscapeCsvField(item.ClassroomName ?? "");
            
            csv.AppendLine($"{item.Id},{escapedClassroomName}");
        }
        
        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);
        
        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);
        
        var fileName = $"Classrooms_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class ClassroomDisplay : TableBase
{
    public string ClassroomName { get; set; } = string.Empty;

    public ClassroomDisplay() : base(0) { }
    public ClassroomDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return ClassroomName;
    }
}