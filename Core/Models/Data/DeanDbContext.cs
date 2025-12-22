using Interfaces.Models.Tables;
using Microsoft.EntityFrameworkCore;
using Models.Models.Tables;
using System.Reflection;

namespace Models.Data;

public class DeanDbContext : DbContext
{
    public DeanDbContext(DbContextOptions<DeanDbContext> options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        
        RegisterAllEntities(modelBuilder);
        
        
        ConfigureTableNames(modelBuilder);
    }
    
    private void RegisterAllEntities(ModelBuilder modelBuilder)
    {
        var entityTypes = Assembly.GetAssembly(typeof(TableBase))?
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ITable).IsAssignableFrom(t))
            .ToList();
        
        if (entityTypes != null)
        {
            foreach (var entityType in entityTypes)
            {
                
                modelBuilder.Entity(entityType);
                
                
                var tableName = entityType.Name;
                modelBuilder.Entity(entityType).ToTable(tableName);
            }
        }
    }
    
    private void ConfigureTableNames(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            
            var tableName = ToSnakeCase(entityType.GetTableName() ?? entityType.ClrType.Name);
            entityType.SetTableName(tableName);
            
            
            foreach (var property in entityType.GetProperties())
            {
                var columnName = ToSnakeCase(property.GetColumnName() ?? property.Name);
                property.SetColumnName(columnName);
            }
        }
    }
    
    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        
        return string.Concat(input.Select((x, i) => 
            i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
}