using Microsoft.EntityFrameworkCore;

namespace TnmsCentralizedDbPlatform.Shared;

public interface ITnmsCentralizedDbPlatform
{
    public const string ModSharpModuleIdentity = "TnmsCentralizedDbPlatform";
    
    DbContextOptionsBuilder<TContext> ConfigureDbContext<TContext>(
        DbConnectionParameters parameters,
        string moduleName) 
        where TContext : DbContext;
}