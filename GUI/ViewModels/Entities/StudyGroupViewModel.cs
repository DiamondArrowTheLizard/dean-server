using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GUI.ViewModels.Entities.Base;
using Interfaces.Services;
using Models.Models.Tables;

namespace GUI.ViewModels.Entities;

public partial class StudyGroupViewModel(IQueryService queryService) : BaseCrudViewModel<StudyGroupDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<Course> _courses = [];

    private async Task LoadCoursesAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllCourses");
            var results = await _queryService.ExecuteQueryAsync<Course>(query);

            Courses.Clear();
            foreach (var course in results)
            {
                Courses.Add(course);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки специальностей: {ex.Message}";
        }
    }

    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await LoadCoursesAsync();
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

    protected override string GetSelectQuery()
    {
        return _queryService.GetQuery("GetAllStudyGroups");
    }

    protected override async Task InsertItemAsync(StudyGroupDisplay item)
    {
        var query = _queryService.GetQuery("InsertStudyGroup");
        var parameters = new Dictionary<string, object>
        {
            ["groupNumber"] = item.GroupNumber,
            ["idCourse"] = item.IdCourse
        };

        var newId = await _queryService.ExecuteScalarAsync<int>(query, parameters);
        if (newId > 0)
        {
            item.Id = newId;
        }
        else
        {
            throw new Exception("Не удалось добавить запись. Возможно, нарушены ограничения БД.");
        }

        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task UpdateItemAsync(StudyGroupDisplay item)
    {
        var query = _queryService.GetQuery("UpdateStudyGroup");
        var parameters = new Dictionary<string, object>
        {
            ["groupNumber"] = item.GroupNumber,
            ["idCourse"] = item.IdCourse,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task DeleteItemAsync(StudyGroupDisplay item)
    {
        var query = _queryService.GetQuery("DeleteStudyGroupWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    private async Task UpdateDisplayFieldsAsync(StudyGroupDisplay item)
    {
        await Task.Delay(0);
        var course = Courses.FirstOrDefault(c => c.Id == item.IdCourse);
        if (course != null)
        {
            item.CourseName = course.CourseName;
        }
    }

    protected override StudyGroupDisplay CreateNewItem()
    {
        var defaultCourse = Courses.FirstOrDefault();

        return new StudyGroupDisplay
        {
            Id = -1,
            GroupNumber = "Новая группа",
            IdCourse = defaultCourse?.Id ?? 1,
            CourseName = defaultCourse?.CourseName ?? "Неизвестная специальность"
        };
    }

    protected override IEnumerable<StudyGroupDisplay> FilterItems(string searchText)
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

            return Items.Where(sg =>
                (sg.GroupNumber != null && sg.GroupNumber.ToLowerInvariant().Contains(searchLower)) ||
                (sg.CourseName != null && sg.CourseName.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;

        var csv = new StringBuilder();

        csv.AppendLine("Id,GroupNumber,IdCourse,CourseName");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"{item.Id},{EscapeCsvField(item.GroupNumber)},{item.IdCourse},{EscapeCsvField(item.CourseName)}");
        }

        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);

        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);

        var fileName = $"StudyGroups_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class StudyGroupDisplay : TableBase
{
    public string GroupNumber { get; set; } = string.Empty;
    public int IdCourse { get; set; }
    public string CourseName { get; set; } = string.Empty;

    public StudyGroupDisplay() : base(0) { }
    public StudyGroupDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{GroupNumber} ({CourseName})";
    }
}