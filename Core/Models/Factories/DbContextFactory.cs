using Interfaces.Factories;
using Interfaces.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Models.Factories;

public class DbContextFactory : IDbContextFactory
{
    private readonly IConnectionInfo _connectionInfo;
    private readonly IServiceProvider _serviceProvider;

    private readonly IDatabaseConnectionString _databaseConnectionString;

    public DbContextFactory(IConnectionInfo connectionInfo,
                            IServiceProvider serviceProvider,
                            IDatabaseConnectionString databaseConnectionString)
    {
        _connectionInfo = connectionInfo;
        _serviceProvider = serviceProvider;
        _databaseConnectionString = databaseConnectionString;
    }

    public DbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

        optionsBuilder.UseNpgsql(_databaseConnectionString.ConnectionString);

        optionsBuilder.UseLazyLoadingProxies();

        return new DbContext(optionsBuilder.Options);
    }
}