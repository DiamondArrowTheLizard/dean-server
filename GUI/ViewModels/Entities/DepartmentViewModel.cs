using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        var query = _queryService.GetQuery("DeleteDepartment");
        var parameters = new Dictionary<string, object>
        {
            ["id"] = item.Id
        };

        await _queryService.ExecuteNonQueryAsync(query, parameters);
    }

    protected override bool IsNewItem(DepartmentDisplay item)
    {
        return item.Id == 0;
    }

    protected override DepartmentDisplay CreateNewItem()
    {

        if (!Faculties.Any())
        {
            _ = LoadFacultiesAsync();
        }

        var selectedFaculty = Faculties.FirstOrDefault();

        return new DepartmentDisplay
        {
            Id = 0,
            DepartmentName = "Новая кафедра",
            IdFaculty = selectedFaculty?.Id ?? 1,
            FacultyName = selectedFaculty?.FacultyName ?? "Неизвестный факультет"
        };
    }

    protected override IEnumerable<DepartmentDisplay> FilterItems(string searchText)
    {
        var searchLower = searchText.ToLowerInvariant();

        return Items.Where(d =>
            (d.DepartmentName != null && d.DepartmentName.ToLowerInvariant().Contains(searchLower)) ||
            (d.FacultyName != null && d.FacultyName.ToLowerInvariant().Contains(searchLower)));
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

public class Faculty : TableBase
{
    public string FacultyName { get; set; } = string.Empty;

    public Faculty() : base(0) { }
    public Faculty(int id) : base(id) { }

    public override string ToString()
    {
        return FacultyName;
    }
}