using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
  public static void Main(string[] args)
  {
    if (args.Length > 0 && args[0].Contains("ef"))
      return;

    try
    {
      Log.Logger = new LoggerConfiguration()
          .WriteTo.Console()
          .CreateLogger();

      Log.Information("Starting web application");

      var builder = WebApplication.CreateBuilder(args);
      builder.AddDefaultLogging();

      ConfigureServices(builder);
      var app = builder.Build();
      ConfigureMiddleware(app);
      ApplyMigrations(app);

      Log.Information("Application configured successfully, starting to listen");

      if (app.Environment.IsDevelopment())
      {
        app.Urls.Clear();
        app.Urls.Add("http://+:8080");
      }

      app.Run();
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "Application terminated unexpectedly");
      throw;
    }
    finally
    {
      Log.CloseAndFlush();
    }
  }

  private static void ConfigureServices(WebApplicationBuilder builder)
  {
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.AddBasicHealthChecks();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<DefaultContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
        )
    );

    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.RegisterDependencies();
    builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

    builder.Services.AddMediatR(cfg =>
    {
      cfg.RegisterServicesFromAssemblies(
              typeof(ApplicationLayer).Assembly,
              typeof(Program).Assembly
          );
    });

    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
  }

  private static void ConfigureMiddleware(WebApplication app)
  {
    app.UseMiddleware<ValidationExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }
    else
    {
      app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseBasicHealthChecks();
    app.MapControllers();
  }

  private static void ApplyMigrations(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

    try
    {
      Log.Information("Applying database migrations...");
      context.Database.Migrate();
      Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Error applying database migrations");
      throw;
    }
  }
}
