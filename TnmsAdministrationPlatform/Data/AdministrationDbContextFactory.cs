using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TnmsAdministrationPlatform.Data;

public class AdministrationDbContextFactory : IDesignTimeDbContextFactory<AdministrationDbContext>
{
    public AdministrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdministrationDbContext>();
        
        optionsBuilder.UseSqlite("Data Source=administration_design.db");
        
        return new AdministrationDbContext(optionsBuilder.Options);
    }
}
