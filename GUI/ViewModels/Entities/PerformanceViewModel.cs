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

public partial class PerformanceViewModel(IQueryService queryService) : BaseCrudViewModel<PerformanceDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<Teacher> _teachers = [];

    [ObservableProperty]
    private ObservableCollection<StudentDisplay> _students = [];

    [ObservableProperty]
    private ObservableCollection<Discipline> _disciplines = [];

    [ObservableProperty]
    private ObservableCollection<KnowledgeCheckType> _knowledgeCheckTypes = [];

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

    private async Task LoadStudentsAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllStudents");
            var results = await _queryService.ExecuteQueryAsync<StudentDisplay>(query);

            Students.Clear();
            foreach (var student in results)
            {
                Students.Add(student);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки студентов: {ex.Message}";
        }
    }

    private async Task LoadDisciplinesAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllDisciplines");
            var results = await _queryService.ExecuteQueryAsync<Discipline>(query);

            Disciplines.Clear();
            foreach (var discipline in results)
            {
                Disciplines.Add(discipline);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки дисциплин: {ex.Message}";
        }
    }

    private async Task LoadKnowledgeCheckTypesAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllKnowledgeCheckTypes");
            var results = await _queryService.ExecuteQueryAsync<KnowledgeCheckType>(query);

            KnowledgeCheckTypes.Clear();
            foreach (var knowledgeCheckType in results)
            {
                KnowledgeCheckTypes.Add(knowledgeCheckType);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки типов проверки знаний: {ex.Message}";
        }
    }

    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await LoadTeachersAsync();
            await LoadStudentsAsync();
            await LoadDisciplinesAsync();
            await LoadKnowledgeCheckTypesAsync();
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
        return _queryService.GetQuery("GetAllPerformances");
    }

    protected override async Task InsertItemAsync(PerformanceDisplay item)
    {
        var query = _queryService.GetQuery("InsertPerformance");
        var parameters = new Dictionary<string, object>
        {
            ["markType"] = item.MarkTypeEnum,
            ["mark"] = item.Mark,
            ["idTeacher"] = item.IdTeacher,
            ["idStudent"] = item.IdStudent
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

    protected override async Task UpdateItemAsync(PerformanceDisplay item)
    {
        var query = _queryService.GetQuery("UpdatePerformance");
        var parameters = new Dictionary<string, object>
        {
            ["markType"] = item.MarkTypeEnum,
            ["mark"] = item.Mark,
            ["idTeacher"] = item.IdTeacher,
            ["idStudent"] = item.IdStudent,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task DeleteItemAsync(PerformanceDisplay item)
    {
        var query = _queryService.GetQuery("DeletePerformanceWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    private async Task UpdateDisplayFieldsAsync(PerformanceDisplay item)
    {
        await Task.Delay(0);
        var teacher = Teachers.FirstOrDefault(t => t.Id == item.IdTeacher);
        if (teacher != null)
        {
            item.TeacherName = $"{teacher.LastName} {teacher.FirstName} {teacher.MiddleName}";
        }

        var student = Students.FirstOrDefault(s => s.Id == item.IdStudent);
        if (student != null)
        {
            item.StudentName = $"{student.LastName} {student.FirstName} {student.MiddleName}";
            item.GroupNumber = student.GroupNumber;
        }
    }

    protected override PerformanceDisplay CreateNewItem()
    {
        var defaultTeacher = Teachers.FirstOrDefault();
        var defaultStudent = Students.FirstOrDefault();

        return new PerformanceDisplay
        {
            Id = -1,
            MarkTypeEnum = "mark",
            Mark = 5,
            IdTeacher = defaultTeacher?.Id ?? 1,
            IdStudent = defaultStudent?.Id ?? 1,
            TeacherName = defaultTeacher != null ? $"{defaultTeacher.LastName} {defaultTeacher.FirstName} {defaultTeacher.MiddleName}" : "Неизвестный преподаватель",
            StudentName = defaultStudent != null ? $"{defaultStudent.LastName} {defaultStudent.FirstName} {defaultStudent.MiddleName}" : "Неизвестный студент",
            GroupNumber = defaultStudent?.GroupNumber ?? "Неизвестная группа"
        };
    }

    protected override IEnumerable<PerformanceDisplay> FilterItems(string searchText)
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

            return Items.Where(p =>
                (p.TeacherName != null && p.TeacherName.ToLowerInvariant().Contains(searchLower)) ||
                (p.StudentName != null && p.StudentName.ToLowerInvariant().Contains(searchLower)) ||
                (p.GroupNumber != null && p.GroupNumber.ToLowerInvariant().Contains(searchLower)) ||
                (p.MarkTypeEnum != null && p.MarkTypeEnum.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;

        var csv = new StringBuilder();

        csv.AppendLine("Id,MarkType,Mark,TeacherName,StudentName,GroupNumber");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"{item.Id},{EscapeCsvField(item.MarkTypeEnum)},{item.Mark},{EscapeCsvField(item.TeacherName)},{EscapeCsvField(item.StudentName)},{EscapeCsvField(item.GroupNumber)}");
        }

        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);

        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);

        var fileName = $"Performances_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class PerformanceDisplay : TableBase
{
    public string MarkTypeEnum { get; set; } = string.Empty;
    public int Mark { get; set; }
    public int IdTeacher { get; set; }
    public int IdStudent { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string GroupNumber { get; set; } = string.Empty;

    public PerformanceDisplay() : base(0) { }
    public PerformanceDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{StudentName}: {Mark} ({MarkTypeEnum})";
    }
}