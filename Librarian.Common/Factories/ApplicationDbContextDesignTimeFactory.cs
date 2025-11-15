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

		var migrationsAssembly = dbType.ToLower() switch
		{
			"sqlite" => "Librarian.Common.Migrations.SQLite",
			"mysql" => "Librarian.Common.Migrations.MySql",
			"postgresql" => "Librarian.Common.Migrations.PostgreSQL",
			_ => throw new ArgumentException(dbType + " is not supported.")
		};

		switch (dbType.ToLower())
        {
            case "sqlite":
				optionsBuilder.UseSqlite(dbConnStr, b => b.MigrationsAssembly(migrationsAssembly));
                break;
            case "mysql":
				optionsBuilder.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr), b => b.MigrationsAssembly(migrationsAssembly));
                break;
            case "postgresql":
				optionsBuilder.UseNpgsql(dbConnStr, b => b.MigrationsAssembly(migrationsAssembly));
                break;
            default:
                throw new ArgumentException(dbType + " is not supported.");
        }

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}