using Microsoft.EntityFrameworkCore;

namespace TnmsCentralizedDbPlatform.Shared;

public interface ITnmsCentralizedDbPlatform
{
    public const string ModSharpModuleIdentity = "TnmsCentralizedDbPlatform";
    
    ITnmsRepository<T> CreateRepository<T>(DbContext context) where T : class;
    
    DbContextOptionsBuilder<TContext> ConfigureDbContext<TContext>(
        DbConnectionParameters parameters,
        string moduleName) 
        where TContext : DbContext;
}