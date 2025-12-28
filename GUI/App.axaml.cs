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
using GUI.ViewModels.RoleMenus.Admin;
using GUI.ViewModels.RoleMenus.Methodist;
using GUI.ViewModels.RoleMenus.ScientificSecretary;
using GUI.ViewModels.RoleMenus.HeadOfDepartment;
using GUI.ViewModels.RoleMenus.Teacher;
using GUI.ViewModels.Entities;
using Models.Services;
using Interfaces.Services;
using GUI.ViewModels.Charts;

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
        collection.AddSingleton<IQueryService, QueryService>();
        collection.AddSingleton<IChartService>(sp =>
        {
            string chartsConnectionString = "Host=localhost;Port=5432;Database=DeanServer;Username=chart_viewer;Password=StrongPassword123!;SSL Mode=Prefer";
            return new ChartService(chartsConnectionString);
        });
        collection.AddSingleton<ChartsViewModel>();

        collection.AddSingleton<MainWindowViewModel>();
        collection.AddSingleton<AuthenticationViewModel>();

        collection.AddTransient<ChangePasswordViewModel>();
        collection.AddTransient<TerminalWindowViewModel>();
        collection.AddTransient<WelcomeScreenViewModel>();

        collection.AddTransient<MenuViewModel>();
        collection.AddTransient<DeanRoleViewModel>();
        collection.AddTransient<AdminRoleViewModel>();
        collection.AddTransient<MethodistRoleViewModel>();
        collection.AddTransient<ScientificSecretaryRoleViewModel>();
        collection.AddTransient<HeadOfDepartmentRoleViewModel>();
        collection.AddTransient<TeacherRoleViewModel>();

        collection.AddTransient<DepartmentViewModel>();
        collection.AddTransient<ClassroomViewModel>();
        collection.AddTransient<StudentViewModel>();
        collection.AddTransient<ScheduleViewModel>();
        collection.AddTransient<PerformanceViewModel>();
        collection.AddTransient<QualificationWorkViewModel>();
        collection.AddTransient<CurriculumViewModel>();
        collection.AddTransient<StudyGroupViewModel>();
        collection.AddTransient<TeacherIndividualPlanViewModel>();
        collection.AddTransient<CityViewModel>();
        collection.AddTransient<StreetViewModel>();
        
        return collection;
    }
}