using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Interfaces.Services;
using GUI.ViewModels.Entities.Base;
using Models.Models.Tables;

namespace GUI.ViewModels.Entities;

public partial class DepartmentViewModel(IQueryService queryService) : BaseCrudViewModel<DepartmentDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<Faculty> _faculties = [];

    private async Task LoadFacultiesAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllFaculties");
            var results = await _queryService.ExecuteQueryAsync<Faculty>(query);

            Faculties.Clear();
            foreach (var faculty in results)
            {
                Faculties.Add(faculty);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки факультетов: {ex.Message}";
        }
    }

    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await LoadFacultiesAsync();
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
        if (!Faculties.Any())
        {
            await LoadFacultiesAsync();
        }

        if (!Faculties.Any())
        {
            StatusMessage = "Невозможно добавить кафедру: нет ни одного факультета.";
            return;
        }

        await base.AddNewAsync();
    }

    protected override string GetSelectQuery()
    {
        return _queryService.GetQuery("GetAllDepartments");
    }

    protected override async Task InsertItemAsync(DepartmentDisplay item)
    {
        var query = _queryService.GetQuery("InsertDepartment");
        var parameters = new Dictionary<string, object>
        {
            ["departmentName"] = item.DepartmentName,
            ["idFaculty"] = item.IdFaculty
        };

        var newId = await _queryService.ExecuteScalarAsync<int>(query, parameters);
        item.Id = newId;

        var faculty = Faculties.FirstOrDefault(f => f.Id == item.IdFaculty);
        if (faculty != null)
        {
            item.FacultyName = faculty.FacultyName;
        }
    }

    protected override async Task UpdateItemAsync(DepartmentDisplay item)
    {
        var query = _queryService.GetQuery("UpdateDepartment");
        var parameters = new Dictionary<string, object>
        {
            ["departmentName"] = item.DepartmentName,
            ["idFaculty"] = item.IdFaculty,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);

        var faculty = Faculties.FirstOrDefault(f => f.Id == item.IdFaculty);
        if (faculty != null)
        {
            item.FacultyName = faculty.FacultyName;
        }
    }

    protected override async Task DeleteItemAsync(DepartmentDisplay item)
    {
        var query = _queryService.GetQuery("DeleteDepartmentWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override DepartmentDisplay CreateNewItem()
    {
        var selectedFaculty = Faculties.FirstOrDefault();

        return new DepartmentDisplay
        {
            Id = -1,
            DepartmentName = "Новая кафедра",
            IdFaculty = selectedFaculty?.Id ?? 1,
            FacultyName = selectedFaculty?.FacultyName ?? "Неизвестный факультет"
        };
    }

    protected override async Task<bool> ConfirmDeleteAsync()
    {
        return await Task.FromResult(true);
    }

    protected override IEnumerable<DepartmentDisplay> FilterItems(string searchText)
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
                (d.DepartmentName != null && d.DepartmentName.ToLowerInvariant().Contains(searchLower)) ||
                (d.FacultyName != null && d.FacultyName.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;
        
        var csv = new StringBuilder();
        
        csv.AppendLine("Id,DepartmentName,IdFaculty,FacultyName");
        
        foreach (var item in itemsToExport)
        {
            var escapedDepartmentName = EscapeCsvField(item.DepartmentName ?? "");
            var escapedFacultyName = EscapeCsvField(item.FacultyName ?? "");
            
            csv.AppendLine($"{item.Id},{escapedDepartmentName},{item.IdFaculty},{escapedFacultyName}");
        }
        
        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);
        
        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);
        
        var fileName = $"Departments_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class DepartmentDisplay : TableBase
{
    public string DepartmentName { get; set; } = string.Empty;
    public int IdFaculty { get; set; }
    public string FacultyName { get; set; } = string.Empty;

    public DepartmentDisplay() : base(0) { }
    public DepartmentDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{DepartmentName} ({FacultyName})";
    }
}