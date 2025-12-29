using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GUI.ViewModels;
using System.Collections.Generic;

namespace GUI.ViewModels.Services;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _selectedFontSizeIndex = 0;
    
    [ObservableProperty]
    private int _selectedThemeIndex = 0;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    [ObservableProperty]
    private bool _showStatusMessage = false;

    
    public List<string> FontSizes { get; } = new()
    {
        "Маленький (10px)",
        "Стандартный (12px)",
        "Большой (14px)",
        "Очень большой (16px)"
    };

    
    public List<string> Themes { get; } = new()
    {
        "Системная",
        "Светлая",
        "Тёмная"
    };

    
    private readonly Dictionary<int, string> _fontSizeKeys = new()
    {
        { 0, "SmallFontSize" },
        { 1, "BaseFontSize" },
        { 2, "LargeFontSize" },
        { 3, "XLargeFontSize" }
    };

    
    private readonly Dictionary<int, string> _themeValues = new()
    {
        { 0, "Default" },
        { 1, "Light" },
        { 2, "Dark" }
    };

    public SettingsViewModel()
    {
        
        LoadSettings();
    }

    [RelayCommand]
    public void ApplySettings()
    {
        ApplyFontSize();
        ApplyTheme();
        
        StatusMessage = "Настройки применены";
        ShowStatusMessage = true;
        
        
        var timer = new System.Threading.Timer(_ =>
        {
            ShowStatusMessage = false;
            OnPropertyChanged(nameof(ShowStatusMessage));
        }, null, 3000, System.Threading.Timeout.Infinite);
    }

    [RelayCommand]
    public void ResetToDefaults()
    {
        SelectedFontSizeIndex = 1; 
        SelectedThemeIndex = 0;    
        ApplySettings();
        StatusMessage = "Настройки сброшены к значениям по умолчанию";
        ShowStatusMessage = true;
    }

    private void ApplyFontSize()
    {
        if (Application.Current == null) return;
        
        
        if (_fontSizeKeys.TryGetValue(SelectedFontSizeIndex, out var resourceKey))
        {
            
            if (Application.Current.Resources.TryGetResource(resourceKey, null, out var value) && value is double fontSize)
            {
                Application.Current.Resources["BaseFontSize"] = fontSize;
                
                
                UpdateMaterialDesignFontSizes(fontSize);
            }
        }
        
        
        SaveSettings();
    }

    private void ApplyTheme()
    {
        if (Application.Current == null) return;
        
        
        if (_themeValues.TryGetValue(SelectedThemeIndex, out var themeValue))
        {
            ThemeVariant theme = themeValue switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
            
            
            Application.Current.Resources["AppThemeVariant"] = theme;
        }
        
        
        SaveSettings();
    }

    private void UpdateMaterialDesignFontSizes(double baseFontSize)
    {
        if (Application.Current == null) return;
        
        
        
        Application.Current.Resources["Material.FontSize.Body1"] = baseFontSize;
        Application.Current.Resources["Material.FontSize.Body2"] = baseFontSize;
        Application.Current.Resources["Material.FontSize.Button"] = baseFontSize;
        Application.Current.Resources["Material.FontSize.Caption"] = baseFontSize * 0.9;
        Application.Current.Resources["Material.FontSize.Overline"] = baseFontSize * 0.8;
        Application.Current.Resources["Material.FontSize.H6"] = baseFontSize * 1.5;
        Application.Current.Resources["Material.FontSize.Subtitle1"] = baseFontSize * 1.2;
    }

    private void LoadSettings()
    {
        
        
        
        
        if (Application.Current != null)
        {
            
            if (Application.Current.Resources.TryGetResource("AppThemeVariant", null, out var currentTheme))
            {
                if (currentTheme is ThemeVariant theme)
                {
                    SelectedThemeIndex = theme.Key switch
                    {
                        "Light" => 1,
                        "Dark" => 2,
                        _ => 0
                    };
                }
            }
        }
    }

    private void SaveSettings()
    {
        
        
        System.Diagnostics.Debug.WriteLine($"Сохранены настройки: FontSize={SelectedFontSizeIndex}, Theme={SelectedThemeIndex}");
    }

    
    [RelayCommand]
    public void OpenFontSettings()
    {
        StatusMessage = "Настройки шрифта открыты";
        ShowStatusMessage = true;
    }

    
    [RelayCommand]
    public void OpenThemeSettings()
    {
        StatusMessage = "Настройки темы открыты";
        ShowStatusMessage = true;
    }
}