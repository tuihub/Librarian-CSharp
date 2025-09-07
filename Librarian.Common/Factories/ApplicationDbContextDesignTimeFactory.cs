using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Librarian.Common.Factories;

public class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        var configuration = configurationBuilder.Build();

        var dbType = configuration["DbType"] ?? "SQLite";
        var dbConnStr = configuration["DbConnStr"] ?? "sephirah.db";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        switch (dbType.ToLower())
        {
            case "sqlite":
                optionsBuilder.UseSqlite(dbConnStr);
                break;
            case "mysql":
                optionsBuilder.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));
                break;
            case "postgresql":
                optionsBuilder.UseNpgsql(dbConnStr);
                break;
            default:
                throw new ArgumentException(dbType + " is not supported.");
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}