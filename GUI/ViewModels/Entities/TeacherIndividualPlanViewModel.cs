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

public partial class TeacherIndividualPlanViewModel(IQueryService queryService) : BaseCrudViewModel<TeacherIndividualPlanDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<Teacher> _teachers = [];

    private async Task LoadTeachersAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllTeachers");
            var results = await _queryService.ExecuteQueryAsync<Teacher>(query);

            Teachers.Clear();
            foreach (var teacher in results)
            {
                Teachers.Add(teacher);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки преподавателей: {ex.Message}";
        }
    }

    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await LoadTeachersAsync();
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
        return _queryService.GetQuery("GetAllTeacherIndividualPlans");
    }

    protected override async Task InsertItemAsync(TeacherIndividualPlanDisplay item)
    {
        var query = _queryService.GetQuery("InsertTeacherIndividualPlan");
        var parameters = new Dictionary<string, object>
        {
            ["totalHours"] = item.TotalHours,
            ["lectureHours"] = item.LectureHours,
            ["practiceHours"] = item.PracticeHours,
            ["labHours"] = item.LabHours,
            ["idTeacher"] = item.IdTeacher
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

    protected override async Task UpdateItemAsync(TeacherIndividualPlanDisplay item)
    {
        var query = _queryService.GetQuery("UpdateTeacherIndividualPlan");
        var parameters = new Dictionary<string, object>
        {
            ["totalHours"] = item.TotalHours,
            ["lectureHours"] = item.LectureHours,
            ["practiceHours"] = item.PracticeHours,
            ["labHours"] = item.LabHours,
            ["idTeacher"] = item.IdTeacher,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task DeleteItemAsync(TeacherIndividualPlanDisplay item)
    {
        var query = _queryService.GetQuery("DeleteTeacherIndividualPlanWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    private async Task UpdateDisplayFieldsAsync(TeacherIndividualPlanDisplay item)
    {
        await Task.Delay(0);
        var teacher = Teachers.FirstOrDefault(t => t.Id == item.IdTeacher);
        if (teacher != null)
        {
            item.TeacherName = $"{teacher.LastName} {teacher.FirstName} {teacher.MiddleName}";
        }
    }

    protected override TeacherIndividualPlanDisplay CreateNewItem()
    {
        var defaultTeacher = Teachers.FirstOrDefault();

        return new TeacherIndividualPlanDisplay
        {
            Id = -1,
            TotalHours = 100,
            LectureHours = 50,
            PracticeHours = 30,
            LabHours = 20,
            IdTeacher = defaultTeacher?.Id ?? 1,
            TeacherName = defaultTeacher != null ? $"{defaultTeacher.LastName} {defaultTeacher.FirstName} {defaultTeacher.MiddleName}" : "Неизвестный преподаватель"
        };
    }

    protected override IEnumerable<TeacherIndividualPlanDisplay> FilterItems(string searchText)
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

            return Items.Where(tip =>
                (tip.TeacherName != null && tip.TeacherName.ToLowerInvariant().Contains(searchLower)) ||
                (tip.TotalHours.ToString().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;

        var csv = new StringBuilder();

        csv.AppendLine("Id,TotalHours,LectureHours,PracticeHours,LabHours,TeacherName");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"{item.Id},{item.TotalHours},{item.LectureHours},{item.PracticeHours},{item.LabHours},{EscapeCsvField(item.TeacherName)}");
        }

        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);

        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);

        var fileName = $"TeacherIndividualPlans_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class TeacherIndividualPlanDisplay : TableBase
{
    public int TotalHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int IdTeacher { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public TeacherIndividualPlanDisplay() : base(0) { }
    public TeacherIndividualPlanDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{TeacherName}: {TotalHours} часов";
    }
}