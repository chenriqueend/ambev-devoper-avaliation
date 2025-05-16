using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.ORM.Context;

public class DeveloperEvaluationContextFactory : IDesignTimeDbContextFactory<DeveloperEvaluationContext>
{
    public DeveloperEvaluationContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DeveloperEvaluationContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseSqlite(
               connectionString,
               b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
        );

        return new DeveloperEvaluationContext(builder.Options);
    }
}