using Microsoft.EntityFrameworkCore;

namespace Interfaces.Factories;
public interface IDbContextFactory
{
    DbContext CreateDbContext();
}