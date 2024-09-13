using Haze.Application.UseCases.Clusters;
using Haze.Infra.Database;
using Haze.Infra.Repositories;
using HazeApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddControllers();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.Section));

builder.Services.AddDbContext<HazeContext>(
    (sp, options) =>
    {
        var databaseOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>();
        options.UseNpgsql(
            $"Server={databaseOptions.Value.Host};Port={databaseOptions.Value.Port};User Id={databaseOptions.Value.Username};Password=admin;Database={databaseOptions.Value.Database};");
    }
);

builder.Services.AddSerilog();

builder.Services.AddScoped<IClusterCommandHandler, ClusterCommandHandler>();

builder.Services.AddScoped<IClustersRepository, ClustersRepository>();

var app = builder.Build();

app.UseHttpMetrics();

app.MapMetrics();

app.MapControllers();

app.Run();