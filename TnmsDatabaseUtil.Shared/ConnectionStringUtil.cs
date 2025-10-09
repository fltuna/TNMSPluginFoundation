using System;
using System.Data.Common;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TnmsDatabaseUtil.Shared;

public static class ConnectionStringUtil
{
    public static DbContextOptionsBuilder<TContext> ConfigureDbContext<TContext>(
        DbConnectionParameters parameters
        ) 
        where TContext : DbContext
    {
        var builder = new DbContextOptionsBuilder<TContext>();
        
        switch (parameters.ProviderType)
        {
            case TnmsDatabaseProviderType.Sqlite:
                var sqliteConnectionString = BuildSqliteConnectionString(parameters);
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

    private static string BuildSqliteConnectionString(DbConnectionParameters parameters)
    {
        var fileName = parameters.Host;

        if (fileName == null || File.GetAttributes(fileName).HasFlag(FileAttributes.Directory))
            throw new InvalidOperationException("you need to specify the full path to the .db file when using SQLite");
        
        return $"Data Source={fileName}";
    }

    private static string BuildMySqlConnectionString(DbConnectionParameters parameters)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ["Server"] = parameters.Host,
            ["Port"] = parameters.Port,
            ["Database"] = parameters.Database,
            ["Uid"] = parameters.Username,
            ["Pwd"] = parameters.Password
        };
        
        foreach (var (key, value) in parameters.AdditionalParameters)
        {
            connectionStringBuilder[key] = value;
        }
        
        return connectionStringBuilder.ToString();
    }
    
    private static string BuildPostgreSqlConnectionString(DbConnectionParameters parameters)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ["Host"] = parameters.Host,
            ["Port"] = parameters.Port,
            ["Database"] = parameters.Database,
            ["Username"] = parameters.Username,
            ["Password"] = parameters.Password
        };
        
        foreach (var (key, value) in parameters.AdditionalParameters)
        {
            connectionStringBuilder[key] = value;
        }
        
        return connectionStringBuilder.ToString();
    }
}