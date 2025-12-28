using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GUI.ViewModels.Help;

public partial class ManualViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _markdownContent = "Загрузка руководства пользователя...";

    public ManualViewModel()
    {
        LoadManualContent();
    }

    private void LoadManualContent()
    {
        try
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Текущая директория: {currentDir}");
            
            var readmePath = Path.Combine(currentDir, "README.md");
            Console.WriteLine($"Путь к README: {readmePath}");
            
            if (!File.Exists(readmePath))
            {
                var projectRoot = Path.Combine(currentDir, "..", "..", "..", "..");
                readmePath = Path.GetFullPath(Path.Combine(projectRoot, "README.md"));
                Console.WriteLine($"Новый путь к README: {readmePath}");
            }
            
            if (File.Exists(readmePath))
            {
                var content = File.ReadAllText(readmePath);
                Console.WriteLine($"Прочитано {content.Length} символов");
                
                var screenshotsDir = Path.Combine(Path.GetDirectoryName(readmePath) ?? "", "Screenshots");
                Console.WriteLine($"Папка Screenshots: {screenshotsDir}");
                Console.WriteLine($"Существует: {Directory.Exists(screenshotsDir)}");
                
                if (Directory.Exists(screenshotsDir))
                {
                    var files = Directory.GetFiles(screenshotsDir, "*.png");
                    Console.WriteLine($"Найдено {files.Length} PNG файлов");
                    
                    foreach (var file in files)
                    {
                        Console.WriteLine($"  {Path.GetFileName(file)}");
                    }
                    
                    content = FixImagePaths(content, Path.GetDirectoryName(readmePath) ?? "");
                }
                
                MarkdownContent = content;
            }
            else
            {
                MarkdownContent = $"Файл руководства не найден.\n\nИскали по пути: {readmePath}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            MarkdownContent = $"Ошибка загрузки руководства: {ex.Message}";
        }
    }

    private string FixImagePaths(string markdownContent, string baseDir)
    {
        var pattern = @"!\[.*?\]\(Screenshots/(.*?)\)";
        
        return Regex.Replace(markdownContent, pattern, match =>
        {
            var fileName = match.Groups[1].Value;
            var fullPath = Path.GetFullPath(Path.Combine(baseDir, "Screenshots", fileName));
            return $"![{match.Groups[0].Value.Split(']')[0].Substring(2)}]({fullPath})";
        });
    }
}