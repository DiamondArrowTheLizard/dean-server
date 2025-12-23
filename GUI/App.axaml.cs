using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using GUI.ViewModels;
using GUI.Views;
using Microsoft.Extensions.DependencyInjection;
using GUI.ViewModels.Authentication;
using Interfaces.Handlers.Authentication;
using Models.Handlers.Authentication;
using Interfaces.Models;
using Models.Models;
using Models.Builders;
using Interfaces.Builders;
using GUI.ViewModels.Shared;
using Interfaces.Handlers.Shared;
using Models.Handlers.Shared;
using GUI.ViewModels.RoleMenus.Dean;
using Interfaces.Factories;
using Interfaces.Handlers.Tables;
using Models.Handlers.Tables;
using System.Reflection;
using Interfaces.Models.Tables;
using Interfaces.Repositories;
using Models.Repositories;
using Models.Data;
using Microsoft.EntityFrameworkCore;
using Models.Models.Tables;
using GUI.ViewModels.RoleMenus.Admin;
using GUI.ViewModels.RoleMenus.Methodist;
using GUI.ViewModels.RoleMenus.ScientificSecretary;
using GUI.ViewModels.RoleMenus.HeadOfDepartment;
using GUI.ViewModels.RoleMenus.Teacher;

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
        
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

private static ServiceCollection ProvideServices()
{
    var collection = new ServiceCollection();

    
    collection.AddSingleton<IConnectionInfo, ConnectionInfo>(
        sp => new ConnectionInfo("localhost", "DeanServer")
    );
    collection.AddSingleton<IDatabaseConnectionString, DatabaseConnectionString>();
    collection.AddSingleton<IConnectionStringBuilder, ConnectionStringBuilder>();
    collection.AddTransient<ITerminalQueryHandler, TerminalQueryHandler>();
    collection.AddTransient<IAuthenticationHandler, AuthenticationHandler>();
    collection.AddTransient<IChangePasswordHandler, ChangePasswordHandler>();
    
    
    collection.AddTransient<IDbContextFactory, Models.Factories.DbContextFactory>(); 
    collection.AddTransient<ITableHandler, TableHandler>();
    
    
    collection.AddScoped<DeanDbContext>(sp =>
    {
        var connectionInfo = sp.GetRequiredService<IConnectionInfo>();
        var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DeanDbContext>();
        
        
        var connectionString = $"Host={connectionInfo.Host};Database={connectionInfo.Database};Username={connectionInfo.Username};Password={connectionInfo.Password}";
        
        optionsBuilder.UseNpgsql(connectionString);
        return new DeanDbContext(optionsBuilder.Options);
    });
    
    
    RegisterTableRepositories(collection);
    
    
    collection.AddSingleton<MainWindowViewModel>();
    collection.AddSingleton<AuthenticationViewModel>();
    collection.AddTransient<ChangePasswordViewModel>();
    collection.AddTransient<TerminalWindowViewModel>();
    collection.AddSingleton<WelcomeScreenViewModel>();

    collection.AddSingleton<MenuViewModel>();
    collection.AddSingleton<DeanRoleViewModel>();
    collection.AddSingleton<AdminRoleViewModel>();
    collection.AddSingleton<MethodistRoleViewModel>();
    collection.AddSingleton<ScientificSecretaryRoleViewModel>();
    collection.AddSingleton<HeadOfDepartmentRoleViewModel>();
    collection.AddSingleton<TeacherRoleViewModel>();

    return collection;
}

private static void RegisterTableRepositories(ServiceCollection collection)
{
    
    var tableTypes = Assembly.GetAssembly(typeof(TableBase))?
        .GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && typeof(ITable).IsAssignableFrom(t))
        .ToList();
    
    if (tableTypes != null)
    {
        foreach (var tableType in tableTypes)
        {
            
            var repositoryType = typeof(ITableRepository<>).MakeGenericType(tableType);
            var implementationType = typeof(TableRepository<>).MakeGenericType(tableType);
            
            collection.AddTransient(repositoryType, implementationType);
        }
    }
}
}