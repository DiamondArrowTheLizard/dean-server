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

public partial class StudentViewModel(IQueryService queryService) : BaseCrudViewModel<StudentDisplay>(queryService)
{
    [ObservableProperty]
    private ObservableCollection<City> _cities = [];

    [ObservableProperty]
    private ObservableCollection<Street> _streets = [];

    [ObservableProperty]
    private ObservableCollection<StudyGroup> _studyGroups = [];

    private async Task LoadCitiesAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllCities");
            var results = await _queryService.ExecuteQueryAsync<City>(query);

            Cities.Clear();
            foreach (var city in results)
            {
                Cities.Add(city);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки городов: {ex.Message}";
        }
    }

    private async Task LoadStreetsAsync()
    {
        try
        {
            var query = _queryService.GetQuery("GetAllStreets");
            var results = await _queryService.ExecuteQueryAsync<Street>(query);

            Streets.Clear();
            foreach (var street in results)
            {
                Streets.Add(street);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки улиц: {ex.Message}";
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

    public override async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";

        try
        {
            await LoadCitiesAsync();
            await LoadStreetsAsync();
            await LoadStudyGroupsAsync();
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
        return _queryService.GetQuery("GetAllStudents");
    }

    protected override async Task InsertItemAsync(StudentDisplay item)
    {
        var query = _queryService.GetQuery("InsertStudent");
        var parameters = new Dictionary<string, object>
        {
            ["lastName"] = item.LastName,
            ["firstName"] = item.FirstName,
            ["middleName"] = item.MiddleName,
            ["gender"] = item.GenderEnum,
            ["birthDate"] = item.BirthDate,
            ["hasChildren"] = item.HasChildren,
            ["phoneNumber"] = item.PhoneNumber,
            ["email"] = item.Email,
            ["snils"] = item.Snils,
            ["studentStatus"] = item.StudentStatusEnum,
            ["educationBasis"] = item.EducationBasisEnum,
            ["enrollmentYear"] = item.EnrollmentYear,
            ["course"] = item.Course,
            ["scholarshipAmount"] = item.ScholarshipAmount,
            ["houseNumber"] = item.HouseNumber,
            ["idStudyGroup"] = item.IdStudyGroup,
            ["idStreet"] = item.IdStreet,
            ["idCity"] = item.IdCity
        };

        var newId = await _queryService.ExecuteScalarAsync<int>(query, parameters);
        item.Id = newId;

        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task UpdateItemAsync(StudentDisplay item)
    {
        var query = _queryService.GetQuery("UpdateStudent");
        var parameters = new Dictionary<string, object>
        {
            ["lastName"] = item.LastName,
            ["firstName"] = item.FirstName,
            ["middleName"] = item.MiddleName,
            ["gender"] = item.GenderEnum,
            ["birthDate"] = item.BirthDate,
            ["hasChildren"] = item.HasChildren,
            ["phoneNumber"] = item.PhoneNumber,
            ["email"] = item.Email,
            ["snils"] = item.Snils,
            ["studentStatus"] = item.StudentStatusEnum,
            ["educationBasis"] = item.EducationBasisEnum,
            ["enrollmentYear"] = item.EnrollmentYear,
            ["course"] = item.Course,
            ["scholarshipAmount"] = item.ScholarshipAmount,
            ["houseNumber"] = item.HouseNumber,
            ["idStudyGroup"] = item.IdStudyGroup,
            ["idStreet"] = item.IdStreet,
            ["idCity"] = item.IdCity,
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
        await UpdateDisplayFieldsAsync(item);
    }

    protected override async Task DeleteItemAsync(StudentDisplay item)
    {
        var query = _queryService.GetQuery("DeleteStudent");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    private async Task UpdateDisplayFieldsAsync(StudentDisplay item)
    {
        await Task.Delay(0);
        var city = Cities.FirstOrDefault(c => c.Id == item.IdCity);
        if (city != null)
        {
            item.CityName = city.CityName;
        }

        var street = Streets.FirstOrDefault(s => s.Id == item.IdStreet);
        if (street != null)
        {
            item.StreetName = street.StreetName;
        }

        var group = StudyGroups.FirstOrDefault(g => g.Id == item.IdStudyGroup);
        if (group != null)
        {
            item.GroupNumber = group.GroupNumber;
        }
    }

    protected override bool IsNewItem(StudentDisplay item)
    {
        return item.Id == 0;
    }

    protected override StudentDisplay CreateNewItem()
    {
        var defaultCity = Cities.FirstOrDefault();
        var defaultStreet = Streets.FirstOrDefault();
        var defaultGroup = StudyGroups.FirstOrDefault();

        return new StudentDisplay
        {
            Id = 0,
            LastName = "Новая",
            FirstName = "Фамилия",
            MiddleName = "Отчество",
            GenderEnum = "male",
            BirthDate = DateTime.Now.AddYears(-20),
            HasChildren = false,
            PhoneNumber = "+79990000000",
            Email = "student@example.com",
            Snils = "000-000-000 00",
            StudentStatusEnum = "active",
            EducationBasisEnum = "budget",
            EnrollmentYear = DateTime.Now,
            Course = 1,
            ScholarshipAmount = 0,
            HouseNumber = "1",
            IdStudyGroup = defaultGroup?.Id ?? 1,
            IdStreet = defaultStreet?.Id ?? 1,
            IdCity = defaultCity?.Id ?? 1,
            CityName = defaultCity?.CityName ?? "Неизвестный город",
            StreetName = defaultStreet?.StreetName ?? "Неизвестная улица",
            GroupNumber = defaultGroup?.GroupNumber ?? "Неизвестная группа",
            CourseName = "Неизвестная специальность",
            FacultyName = "Неизвестный факультет"
        };
    }

    protected override IEnumerable<StudentDisplay> FilterItems(string searchText)
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
                (s.LastName != null && s.LastName.ToLowerInvariant().Contains(searchLower)) ||
                (s.FirstName != null && s.FirstName.ToLowerInvariant().Contains(searchLower)) ||
                (s.MiddleName != null && s.MiddleName.ToLowerInvariant().Contains(searchLower)) ||
                (s.PhoneNumber != null && s.PhoneNumber.ToLowerInvariant().Contains(searchLower)) ||
                (s.Email != null && s.Email.ToLowerInvariant().Contains(searchLower)) ||
                (s.Snils != null && s.Snils.ToLowerInvariant().Contains(searchLower)) ||
                (s.CityName != null && s.CityName.ToLowerInvariant().Contains(searchLower)) ||
                (s.StreetName != null && s.StreetName.ToLowerInvariant().Contains(searchLower)) ||
                (s.GroupNumber != null && s.GroupNumber.ToLowerInvariant().Contains(searchLower)) ||
                (s.CourseName != null && s.CourseName.ToLowerInvariant().Contains(searchLower)) ||
                (s.FacultyName != null && s.FacultyName.ToLowerInvariant().Contains(searchLower)));
        }
    }

    protected override Task<string> ExportDataAsync()
    {
        var itemsToExport = FilteredItems.Any() ? FilteredItems : Items;

        var csv = new StringBuilder();

        csv.AppendLine("Id,LastName,FirstName,MiddleName,Gender,BirthDate,HasChildren,PhoneNumber,Email,Snils,StudentStatus,EducationBasis,EnrollmentYear,Course,ScholarshipAmount,HouseNumber,GroupNumber,StreetName,CityName,CourseName,FacultyName");

        foreach (var item in itemsToExport)
        {
            csv.AppendLine($"{item.Id},{EscapeCsvField(item.LastName)},{EscapeCsvField(item.FirstName)},{EscapeCsvField(item.MiddleName)},{EscapeCsvField(item.GenderEnum)},{item.BirthDate:yyyy-MM-dd},{item.HasChildren},{EscapeCsvField(item.PhoneNumber)},{EscapeCsvField(item.Email)},{EscapeCsvField(item.Snils)},{EscapeCsvField(item.StudentStatusEnum)},{EscapeCsvField(item.EducationBasisEnum)},{item.EnrollmentYear:yyyy-MM-dd},{item.Course},{item.ScholarshipAmount},{EscapeCsvField(item.HouseNumber)},{EscapeCsvField(item.GroupNumber)},{EscapeCsvField(item.StreetName)},{EscapeCsvField(item.CityName)},{EscapeCsvField(item.CourseName)},{EscapeCsvField(item.FacultyName)}");
        }

        return Task.FromResult(csv.ToString());
    }

    protected override async Task SaveExportFileAsync(string data)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionRoot = FindSolutionRoot(currentDirectory);

        var folderPath = Path.Combine(solutionRoot, "SavedTables");
        Directory.CreateDirectory(folderPath);

        var fileName = $"Students_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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

public class StudentDisplay : TableBase
{
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string GenderEnum { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public bool HasChildren { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Snils { get; set; } = string.Empty;
    public string StudentStatusEnum { get; set; } = string.Empty;
    public string EducationBasisEnum { get; set; } = string.Empty;
    public DateTime EnrollmentYear { get; set; }
    public int Course { get; set; }
    public int ScholarshipAmount { get; set; }
    public string HouseNumber { get; set; } = string.Empty;
    public int IdStudyGroup { get; set; }
    public int IdStreet { get; set; }
    public int IdCity { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string GroupNumber { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;

    public StudentDisplay() : base(0) { }
    public StudentDisplay(int id) : base(id) { }

    public override string ToString()
    {
        return $"{LastName} {FirstName} {MiddleName} ({GroupNumber})";
    }
}