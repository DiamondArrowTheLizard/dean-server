using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using GUI.ViewModels;
using GUI.Views;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using Microsoft.VisualBasic;
using GUI.ViewModels.Authentication;
using GUI.Views.Authentication;
using Interfaces.Handlers.Authentication;
using Models.Handlers.Authentication;

namespace GUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _ = new ServiceCollection();
            ServiceCollection? serviceCollection = ProvideServices();

            var services = serviceCollection.BuildServiceProvider();
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private static ServiceCollection ProvideServices()
    {
        var collection = new ServiceCollection();

        collection.AddSingleton<IAuthenticationHandler, AuthenticationHandler>();
        collection.AddSingleton<IChangePassowrdHandler, ChangePassowrdHandler>();

        collection.AddSingleton<MainWindowViewModel>();

        collection.AddSingleton<AuthenticationViewModel>();
        collection.AddSingleton<ChangePasswordViewModel>();
        collection.AddSingleton<AuthenticationButtonViewModel>();
        collection.AddSingleton<ChangePasswordButtonViewModel>();




        return collection;
    }
}