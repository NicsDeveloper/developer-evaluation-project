using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Integration;

/// <summary>
/// Configuração de startup para testes de integração
/// </summary>
public class TestStartup
{
    /// <summary>
    /// Configura os serviços necessários para os testes
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    public static void ConfigureServices(IServiceCollection services)
    {
        // Configurar logging para testes
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Adicionar controllers com assembly explícito
        services.AddControllers()
            .AddApplicationPart(typeof(Ambev.DeveloperEvaluation.WebApi.Features.Sales.SalesController).Assembly);

        // Configurar banco de dados em memória para testes
        // Usar um nome único para cada teste para garantir isolamento
        var databaseName = Guid.NewGuid().ToString();
        services.AddDbContext<DefaultContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName)
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors();
        });

        // Registrar repositórios
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Registrar MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly
            );
        });

        // Configurar AutoMapper se necessário
        services.AddAutoMapper(typeof(ApplicationLayer).Assembly);
    }

    /// <summary>
    /// Configura o pipeline de requisições HTTP
    /// </summary>
    /// <param name="app">Builder da aplicação</param>
    /// <param name="env">Ambiente de hospedagem</param>
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configurar roteamento
        app.UseRouting();

        // Configurar endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Configurar middleware de tratamento de exceções para testes
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                // Log da exceção para debug dos testes
                var logger = context.RequestServices.GetRequiredService<ILogger<TestStartup>>();
                logger.LogError(ex, "Unhandled exception during test execution");
                throw;
            }
        });
    }
}
