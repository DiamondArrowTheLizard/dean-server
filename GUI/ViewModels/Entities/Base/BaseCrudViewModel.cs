using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces.Services;

namespace GUI.ViewModels.Entities.Base;

public abstract partial class BaseCrudViewModel<T>(IQueryService queryService) : ViewModelBase where T : class, new()
{
    protected readonly IQueryService _queryService = queryService;
    
    [ObservableProperty]
    private ObservableCollection<T> _items = [];
    
    [ObservableProperty]
    private ObservableCollection<T> _filteredItems = [];
    
    [ObservableProperty]
    private T? _selectedItem;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = "";
    
    [ObservableProperty]
    private string _searchText = "";

    [RelayCommand]
    public virtual async Task LoadDataAsync()
    {
        IsLoading = true;
        StatusMessage = "Загрузка данных...";
        
        try
        {
            var query = GetSelectQuery();
            var results = await _queryService.ExecuteQueryAsync<T>(query);
            
            Items.Clear();
            FilteredItems.Clear();
            
            foreach (var item in results)
            {
                Items.Add(item);
                FilteredItems.Add(item);
            }
            
            StatusMessage = $"Загружено {Items.Count} записей";
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Строка подключения не установлена"))
        {
            StatusMessage = "Ошибка: не выполнена аутентификация";
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
    
    [RelayCommand]
    public virtual async Task AddNewAsync()
    {
        await Task.Delay(0);
        try
        {
            var newItem = CreateNewItem();
            Items.Add(newItem);
            FilteredItems.Add(newItem);
            SelectedItem = newItem;
            
            StatusMessage = "Новая запись добавлена. Сохраните изменения.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка добавления: {ex.Message}";
        }
    }
    
    [RelayCommand]
    public virtual async Task SaveAsync()
    {
        if (SelectedItem == null)
        {
            StatusMessage = "Не выбрана запись для сохранения";
            return;
        }
        
        IsLoading = true;
        StatusMessage = "Сохранение данных...";
        
        try
        {
            if (IsNewItem(SelectedItem))
            {
                await InsertItemAsync(SelectedItem);
                StatusMessage = "Запись успешно добавлена";
            }
            else
            {
                await UpdateItemAsync(SelectedItem);
                StatusMessage = "Запись успешно обновлена";
            }
            
            await LoadDataAsync();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Строка подключения не установлена"))
        {
            StatusMessage = "Ошибка: не выполнена аутентификация";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сохранения: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    public virtual async Task DeleteAsync()
    {
        if (SelectedItem == null)
        {
            StatusMessage = "Не выбрана запись для удаления";
            return;
        }
        
        if (!await ConfirmDeleteAsync())
        {
            return;
        }
        
        IsLoading = true;
        StatusMessage = "Удаление записи...";
        
        try
        {
            await DeleteItemAsync(SelectedItem);
            Items.Remove(SelectedItem);
            FilteredItems.Remove(SelectedItem);
            SelectedItem = null;
            
            StatusMessage = "Запись успешно удалена";
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Строка подключения не установлена"))
        {
            StatusMessage = "Ошибка: не выполнена аутентификация";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка удаления: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    public virtual async Task ExportAsync()
    {
        try
        {
            var exportData = await ExportDataAsync();
            await SaveExportFileAsync(exportData);
            StatusMessage = "Данные экспортированы";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка экспорта: {ex.Message}";
        }
    }
    
    [RelayCommand]
    public virtual async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            
            FilteredItems.Clear();
            foreach (var item in Items)
            {
                FilteredItems.Add(item);
            }
            StatusMessage = $"Показаны все {FilteredItems.Count} записей";
            return;
        }
        
        try
        {
            var filtered = FilterItems(SearchText).ToList();
            FilteredItems.Clear();
            
            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
            }
            
            StatusMessage = $"Найдено {FilteredItems.Count} записей";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка поиска: {ex.Message}";
        }
    }
    
    
    protected abstract string GetSelectQuery();
    protected abstract Task InsertItemAsync(T item);
    protected abstract Task UpdateItemAsync(T item);
    protected abstract Task DeleteItemAsync(T item);
    
    protected virtual T CreateNewItem() => new T();
    protected virtual bool IsNewItem(T item) => true;
    protected virtual Task<bool> ConfirmDeleteAsync() => Task.FromResult(true);
    
    protected virtual IEnumerable<T> FilterItems(string searchText)
    {
        return Items.Where(item =>
            item.ToString()?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false);
    }
    
    protected virtual Task<string> ExportDataAsync() => Task.FromResult(string.Join("\n", Items.Select(i => i.ToString())));
    protected virtual Task SaveExportFileAsync(string data)
    {
        return Task.CompletedTask;
    }
}