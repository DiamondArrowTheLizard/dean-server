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

public partial class ScheduleViewModel(IQueryService queryService) : BaseCrudViewModel<ScheduleDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<StudyGroup> _studyGroups = [];

    [ObservableProperty]
    private ObservableCollection<Discipline> _disciplines = [];

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

    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await LoadStudyGroupsAsync();
            await LoadDisciplinesAsync();
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
        return _queryService.GetQuery("GetAllSchedules");
    }

    protected override async Task InsertItemAsync(ScheduleDisplay item)
    {
        var query = _queryService.GetQuery("InsertSchedule");
        var parameters = new Dictionary<string, object>
        {
            ["studyWeeks"] = item.StudyWeeks,
            ["dayOfWeek"] = item.DayOfWeekEnum,
            ["startTime"] = item.StartTime,
            ["endTime"] = item.EndTime,
            ["classType"] = item.ClassTypeEnum,
            ["idStudyGroup"] = item.IdStudyGroup,
            ["idDiscipline"] = item.IdDiscipline
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

    protected override async Task UpdateItemAsync(ScheduleDisplay item)
    {
        var query = _queryService.GetQuery("UpdateSchedule");
        var parameters = new Dictionary<string, object>
        {
            ["studyWeeks"] = item.StudyWeeks,
            ["dayOfWeek"] = item.DayOfWeekEnum,
            ["startTime"] = item.StartTime,
            ["endTime"] = item.EndTime,
            ["classType"] = item.ClassTypeEnum,
            ["idStudyGroup"] = item.IdStudyGroup,
            ["idDiscipline"] = item.IdDiscipline,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task DeleteItemAsync(ScheduleDisplay item)
    {
        var query = _queryService.GetQuery("DeleteScheduleWithDependencies");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    private async Task UpdateDisplayFieldsAsync(ScheduleDisplay item)
    {
        await Task.Delay(0);
        var group = StudyGroups.FirstOrDefault(g => g.Id == item.IdStudyGroup);
        if (group != null)
        {
            item.GroupNumber = group.GroupNumber;
        }

        var discipline = Disciplines.FirstOrDefault(d => d.Id == item.IdDiscipline);
        if (discipline != null)
        {
            item.DisciplineName = discipline.DisciplineName;
        }
    }

    protected override ScheduleDisplay CreateNewItem()
    {
        var defaultGroup = StudyGroups.FirstOrDefault();
        var defaultDiscipline = Disciplines.FirstOrDefault();

        return new ScheduleDisplay
        {
            Id = -1,
            StudyWeeks = 1,
            DayOfWeekEnum = "monday",
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            ClassTypeEnum = "lecture",
            IdStudyGroup = defaultGroup?.Id ?? 1,
            IdDiscipline = defaultDiscipline?.Id ?? 1,
            GroupNumber = defaultGroup?.GroupNumber ?? "Неизвестная группа",
            DisciplineName = defaultDiscipline?.DisciplineName ?? "Неизвестная дисциплина"
        };
    }

    protected override IEnumerable<ScheduleDisplay> FilterItems(string searchText)
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

            return Items.Where(s =>
                (s.GroupNumber != null && s.GroupNumber.ToLowerInvariant().Contains(searchLower)) ||
                (s.DisciplineName != null && s.DisciplineName.ToLowerInvariant().Contains(searchLower)) ||
                (s.DayOfWeekEnum != null && s.DayOfWeekEnum.ToLowerInvariant().Contains(searchLower)) ||
                (s.ClassTypeEnum != null && s.ClassTypeEnum.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;

        var csv = new StringBuilder();

        csv.AppendLine("Id,StudyWeeks,DayOfWeek,StartTime,EndTime,ClassType,GroupNumber,DisciplineName");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"{item.Id},{item.StudyWeeks},{EscapeCsvField(item.DayOfWeekEnum)},{item.StartTime:hh\\:mm},{item.EndTime:hh\\:mm},{EscapeCsvField(item.ClassTypeEnum)},{EscapeCsvField(item.GroupNumber)},{EscapeCsvField(item.DisciplineName)}");
        }

        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);

        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);

        var fileName = $"Schedules_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class ScheduleDisplay : TableBase
{
    public int StudyWeeks { get; set; }
    public string DayOfWeekEnum { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string ClassTypeEnum { get; set; } = string.Empty;
    public int IdStudyGroup { get; set; }
    public int IdDiscipline { get; set; }
    public string GroupNumber { get; set; } = string.Empty;
    public string DisciplineName { get; set; } = string.Empty;

    public ScheduleDisplay() : base(0) { }
    public ScheduleDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{DayOfWeekEnum} {StartTime:hh\\:mm}-{EndTime:hh\\:mm} ({GroupNumber} - {DisciplineName})";
    }
}