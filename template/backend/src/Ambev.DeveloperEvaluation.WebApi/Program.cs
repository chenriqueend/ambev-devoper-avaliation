using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Context;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            Console.WriteLine("Builder created");

            builder.AddDefaultLogging();
            Console.WriteLine("Logging configured");

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();
            Console.WriteLine("Basic services configured");

            builder.Services.AddInfrastructure(builder.Configuration);
            Console.WriteLine("Infrastructure configured");

            builder.Services.AddJwtAuthentication(builder.Configuration);
            Console.WriteLine("Authentication configured");

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);
            Console.WriteLine("AutoMapper configured");

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });
            Console.WriteLine("MediatR configured");

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var app = builder.Build();
            Console.WriteLine("Application built");

            // Configure SQLite WAL mode
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DeveloperEvaluationContext>();
                // Aplica as migrations automaticamente
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

            app.UseMiddleware<ValidationExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();
            Console.WriteLine("Swagger configured");

            app.UseAuthentication();
            app.UseAuthorization();
            Console.WriteLine("Auth middleware configured");

            app.UseBasicHealthChecks();
            Console.WriteLine("Health checks configured");

            app.MapControllers();
            Console.WriteLine("Controllers mapped");

            Console.WriteLine("Starting server...");
            app.Run();
            Console.WriteLine("Server started");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application terminated unexpectedly: {ex}");
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
