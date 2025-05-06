using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Handlers.Sales;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Context;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<DeveloperEvaluationContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"), sqliteOptions =>
                {
                    sqliteOptions.CommandTimeout(30);
                });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            });

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Security
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Validators
            services.AddScoped<UserValidator>();

            // Handlers
            services.AddScoped<IRequestHandler<CreateSaleCommand, CommandResult<Guid>>, CreateSaleCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateSaleItemCommand, CommandResult<bool>>, UpdateSaleItemCommandHandler>();
            services.AddScoped<IRequestHandler<CancelSaleCommand, CommandResult<bool>>, CancelSaleCommandHandler>();
            services.AddScoped<IRequestHandler<CancelSaleItemCommand, CommandResult<bool>>, CancelSaleItemCommandHandler>();

            return services;
        }
    }
}