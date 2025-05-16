using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM.Context;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        bool runMigrationsOnly = args.Contains("--migrate");

        // Remove o --migrate dos args para o builder não se perder
        var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--migrate").ToArray());

        builder.AddDefaultLogging();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.AddBasicHealthChecks();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DeveloperEvaluationContext>();
            context.Database.Migrate();

            var connection = context.Database.GetDbConnection() as SqliteConnection;
            if (connection != null)
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA journal_mode = WAL;";
                command.ExecuteNonQuery();
            }
        }

        if (runMigrationsOnly)
        {
            Console.WriteLine("✅ Migrations done.");
            return;
        }

        app.UseMiddleware<ValidationExceptionMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseBasicHealthChecks();

        app.MapControllers();

        app.Run();
    }
}
