using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfaces.Services;

namespace GUI.ViewModels.Charts
{
    public partial class ChartsViewModel(IChartService chartService) : ViewModelBase
    {
        private readonly IChartService _chartService = chartService;
        
        [ObservableProperty]
        private Bitmap? _chartImage;
        
        [ObservableProperty]
        private string _currentImagePath = string.Empty;
        
        [ObservableProperty]
        private string _statusMessage = "Нажмите 'Обновить графики' для генерации";
        
        [ObservableProperty]
        private bool _isLoading;

        [RelayCommand]
        private async Task GenerateChartsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Генерация графиков...";
                
                var imagePath = await _chartService.GenerateChartsAsync();
                
                CurrentImagePath = imagePath;
                
                using (var imageStream = System.IO.File.OpenRead(imagePath))
                {
                    ChartImage = new Bitmap(imageStream);
                }
                
                StatusMessage = "Графики успешно сгенерированы";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task SaveAsPngAsync()
        {
            if (string.IsNullOrEmpty(CurrentImagePath) || !System.IO.File.Exists(CurrentImagePath))
            {
                StatusMessage = "Сначала сгенерируйте графики";
                return;
            }
            
            var topLevel = GetTopLevel();
            if (topLevel == null) return;
            
            var files = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить графики как PNG",
                DefaultExtension = ".png",
                SuggestedFileName = "графики_деканата",
                FileTypeChoices =
                [
                    new FilePickerFileType("PNG Image") { Patterns = ["*.png"] }
                ]
            });
            
            if (files != null)
            {
                try
                {
                    System.IO.File.Copy(CurrentImagePath, files.Path.LocalPath, overwrite: true);
                    StatusMessage = $"Графики сохранены: {files.Name}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка сохранения: {ex.Message}";
                }
            }
        }
        
        [RelayCommand]
        private async Task SaveAsPdfAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Генерация PDF...";
                
                var pdfPath = await _chartService.GenerateChartsPdfAsync();
                
                var topLevel = GetTopLevel();
                if (topLevel == null) return;
                
                var files = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Сохранить графики как PDF",
                    DefaultExtension = ".pdf",
                    SuggestedFileName = "графики_деканата",
                    FileTypeChoices =
                    [
                        new FilePickerFileType("PDF Document") { Patterns = ["*.pdf"] }
                    ]
                });
                
                if (files != null)
                {
                    System.IO.File.Copy(pdfPath, files.Path.LocalPath, overwrite: true);
                    StatusMessage = $"PDF сохранен: {files.Name}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private static TopLevel? GetTopLevel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }
    }
}