using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Objects;
using TnmsCentralizedDbPlatform.Shared;

namespace TnmsCentralizedDbPlatform;

public class TnmsCentralizedDbPlatform : IModSharpModule, ITnmsCentralizedDbPlatform
{
    private readonly ILogger _logger;
    private readonly ISharedSystem _sharedSystem;
    private readonly string? _sharpPath;
    
    public TnmsCentralizedDbPlatform(ISharedSystem sharedSystem,
        string? dllPath,
        string? sharpPath,
        Version? version,
        IConfiguration? coreConfiguration,
        bool hotReload)
    {
        _sharedSystem = sharedSystem;
        _sharpPath = sharpPath;
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<TnmsCentralizedDbPlatform>();
        
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
    }
    
    public string DisplayName => "TnmsCentralizedDbPlatform";
    public string DisplayAuthor => "faketuna";
    
    public bool Init()
    {
        _logger.LogInformation("Loaded TnmsCentralizedDbPlatform");
        return true;
    }

    public void PostInit()
    {
        _sharedSystem.GetSharpModuleManager().RegisterSharpModuleInterface(this, ITnmsCentralizedDbPlatform.ModSharpModuleIdentity, (ITnmsCentralizedDbPlatform)this);
    }

    public void Shutdown()
    {
        _logger.LogInformation("Unloaded TnmsCentralizedDbPlatform");
    }
    
    public ITnmsRepository<T> CreateRepository<T>(DbContext context) where T : class
    {
        return new TnmsRepository<T>(context);
    }

    public DbContextOptionsBuilder<TContext> ConfigureDbContext<TContext>(
        DbConnectionParameters parameters,
        string moduleName) 
        where TContext : DbContext
    {
        var builder = new DbContextOptionsBuilder<TContext>();
        
        switch (parameters.ProviderType)
        {
            case TnmsDatabaseProviderType.Sqlite:
                var sqliteConnectionString = BuildSqliteConnectionString(parameters, moduleName);
                _logger.LogInformation($"SQLite connection string: {sqliteConnectionString}");
                builder.UseSqlite(sqliteConnectionString);
                break;
                
            case TnmsDatabaseProviderType.MySql:
                var mysqlConnectionString = BuildMySqlConnectionString(parameters);
                builder.UseMySql(mysqlConnectionString, ServerVersion.AutoDetect(mysqlConnectionString));
                break;
                
            case TnmsDatabaseProviderType.PostgreSql:
                var postgresConnectionString = BuildPostgreSqlConnectionString(parameters);
                builder.UseNpgsql(postgresConnectionString);
                break;
                
            default:
                throw new NotSupportedException($"Database provider {parameters.ProviderType} is not supported");
        }
        
        return builder;
    }

    private string BuildSqliteConnectionString(DbConnectionParameters parameters, string moduleName)
    {
        var fileName = parameters.Host ?? "data.db";
        
        // If it's just a filename without path, place it in the specified module's directory
        if (!Path.IsPathRooted(fileName) && !fileName.Contains(Path.DirectorySeparatorChar))
        {
            string moduleDirectory;
            if (!string.IsNullOrEmpty(_sharpPath))
            {
                // Place in specific module directory: /sharp/modules/{moduleName}/
                moduleDirectory = Path.Combine(_sharpPath, "modules", moduleName);
            }
            else
            {
                // Fallback to core data directory
                moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            }
            
            Directory.CreateDirectory(moduleDirectory);
            fileName = Path.Combine(moduleDirectory, fileName);
            
            _logger.LogInformation($"SQLite database will be created at: {fileName}");
        }
        
        return $"Data Source={fileName}";
    }

    private static string BuildMySqlConnectionString(DbConnectionParameters parameters)
    {
        var connectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder
        {
            ["Server"] = parameters.Host,
            ["Port"] = parameters.Port,
            ["Database"] = parameters.Database,
            ["Uid"] = parameters.Username,
            ["Pwd"] = parameters.Password
        };
        
        return connectionStringBuilder.ConnectionString;
    }
    
    private static string BuildPostgreSqlConnectionString(DbConnectionParameters parameters)
    {
        var connectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder
        {
            ["Host"] = parameters.Host,
            ["Port"] = parameters.Port,
            ["Database"] = parameters.Database,
            ["Username"] = parameters.Username,
            ["Password"] = parameters.Password
        };
        
        return connectionStringBuilder.ConnectionString;
    }
}
