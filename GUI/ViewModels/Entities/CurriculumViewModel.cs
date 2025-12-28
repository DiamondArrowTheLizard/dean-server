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

public partial class CurriculumViewModel(IQueryService queryService) : BaseCrudViewModel<CurriculumDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<Discipline> _disciplines = [];

    [ObservableProperty]
    private ObservableCollection<Course> _courses = [];

    [ObservableProperty]
    private ObservableCollection<StudyGroup> _studyGroups = [];

    [ObservableProperty]
    private ObservableCollection<KnowledgeCheckType> _knowledgeCheckTypes = [];

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

    private async Task LoadStudyGroupsAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllStudyGroups");
            var results = await _queryService.ExecuteQueryAsync<StudyGroup>(query);

            StudyGroups.Clear();
            foreach (var group in results)
            {
                StudyGroups.Add(group);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки групп: {ex.Message}";
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
            await LoadDisciplinesAsync();
            await LoadCoursesAsync();
            await LoadStudyGroupsAsync();
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
        return _queryService.GetQuery("GetAllCurriculums");
    }

    protected override async Task InsertItemAsync(CurriculumDisplay item)
    {
        var query = _queryService.GetQuery("InsertCurriculum");
        var parameters = new Dictionary<string, object>
        {
            ["totalHours"] = item.TotalHours,
            ["lectureHours"] = item.LectureHours,
            ["practiceHours"] = item.PracticeHours,
            ["labHours"] = item.LabHours,
            ["idDiscipline"] = item.IdDiscipline,
            ["idCourse"] = item.IdCourse,
            ["idStudyGroup"] = item.IdStudyGroup,
            ["idKnowledgeCheckType"] = item.IdKnowledgeCheckType
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

    protected override async Task UpdateItemAsync(CurriculumDisplay item)
    {
        var query = _queryService.GetQuery("UpdateCurriculum");
        var parameters = new Dictionary<string, object>
        {
            ["totalHours"] = item.TotalHours,
            ["lectureHours"] = item.LectureHours,
            ["practiceHours"] = item.PracticeHours,
            ["labHours"] = item.LabHours,
            ["idDiscipline"] = item.IdDiscipline,
            ["idCourse"] = item.IdCourse,
            ["idStudyGroup"] = item.IdStudyGroup,
            ["idKnowledgeCheckType"] = item.IdKnowledgeCheckType,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task DeleteItemAsync(CurriculumDisplay item)
    {
        var query = _queryService.GetQuery("DeleteCurriculumWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    private async Task UpdateDisplayFieldsAsync(CurriculumDisplay item)
    {
        await Task.Delay(0);
        var discipline = Disciplines.FirstOrDefault(d => d.Id == item.IdDiscipline);
        if (discipline != null)
        {
            item.DisciplineName = discipline.DisciplineName;
        }

        var course = Courses.FirstOrDefault(c => c.Id == item.IdCourse);
        if (course != null)
        {
            item.CourseName = course.CourseName;
        }

        var group = StudyGroups.FirstOrDefault(g => g.Id == item.IdStudyGroup);
        if (group != null)
        {
            item.GroupNumber = group.GroupNumber;
        }

        var knowledgeCheckType = KnowledgeCheckTypes.FirstOrDefault(k => k.Id == item.IdKnowledgeCheckType);
        if (knowledgeCheckType != null)
        {
            item.KnowledgeCheckTypeEnum = knowledgeCheckType.KnowledgeCheckTypeEnum;
        }
    }

    protected override CurriculumDisplay CreateNewItem()
    {
        var defaultDiscipline = Disciplines.FirstOrDefault();
        var defaultCourse = Courses.FirstOrDefault();
        var defaultGroup = StudyGroups.FirstOrDefault();
        var defaultKnowledgeCheckType = KnowledgeCheckTypes.FirstOrDefault();

        return new CurriculumDisplay
        {
            Id = -1,
            TotalHours = 100,
            LectureHours = 50,
            PracticeHours = 30,
            LabHours = 20,
            IdDiscipline = defaultDiscipline?.Id ?? 1,
            IdCourse = defaultCourse?.Id ?? 1,
            IdStudyGroup = defaultGroup?.Id ?? 1,
            IdKnowledgeCheckType = defaultKnowledgeCheckType?.Id ?? 1,
            DisciplineName = defaultDiscipline?.DisciplineName ?? "Неизвестная дисциплина",
            CourseName = defaultCourse?.CourseName ?? "Неизвестная специальность",
            GroupNumber = defaultGroup?.GroupNumber ?? "Неизвестная группа",
            KnowledgeCheckTypeEnum = defaultKnowledgeCheckType?.KnowledgeCheckTypeEnum ?? "Неизвестный тип"
        };
    }

    protected override IEnumerable<CurriculumDisplay> FilterItems(string searchText)
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

            return Items.Where(c =>
                (c.DisciplineName != null && c.DisciplineName.ToLowerInvariant().Contains(searchLower)) ||
                (c.CourseName != null && c.CourseName.ToLowerInvariant().Contains(searchLower)) ||
                (c.GroupNumber != null && c.GroupNumber.ToLowerInvariant().Contains(searchLower)) ||
                (c.KnowledgeCheckTypeEnum != null && c.KnowledgeCheckTypeEnum.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;

        var csv = new StringBuilder();

        csv.AppendLine("Id,TotalHours,LectureHours,PracticeHours,LabHours,DisciplineName,CourseName,GroupNumber,KnowledgeCheckType");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"{item.Id},{item.TotalHours},{item.LectureHours},{item.PracticeHours},{item.LabHours},{EscapeCsvField(item.DisciplineName)},{EscapeCsvField(item.CourseName)},{EscapeCsvField(item.GroupNumber)},{EscapeCsvField(item.KnowledgeCheckTypeEnum)}");
        }

        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);

        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);

        var fileName = $"Curriculums_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class CurriculumDisplay : TableBase
{
    public int TotalHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int IdDiscipline { get; set; }
    public int IdCourse { get; set; }
    public int IdStudyGroup { get; set; }
    public int IdKnowledgeCheckType { get; set; }
    public string DisciplineName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string GroupNumber { get; set; } = string.Empty;
    public string KnowledgeCheckTypeEnum { get; set; } = string.Empty;

    public CurriculumDisplay() : base(0) { }
    public CurriculumDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{DisciplineName} ({GroupNumber}) - {TotalHours} часов";
    }
}