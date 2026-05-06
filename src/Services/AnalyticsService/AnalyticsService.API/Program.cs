using BuildingBlocks.Common.Middleware;
using BuildingBlocks.Security.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AnalyticsService.Application.Services;
using AnalyticsService.Domain.Repositories;
using AnalyticsService.Infrastructure.Consumers;
using AnalyticsService.Infrastructure.Data;
using AnalyticsService.Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("AnalyticsDb"),
        npgsql => npgsql.MigrationsAssembly("AnalyticsService.Infrastructure")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "AnalyticsService_";
});

builder.Services.AddScoped<IJobStatRepository, JobStatRepository>();
builder.Services.AddScoped<IPlatformStatRepository, PlatformStatRepository>();
builder.Services.AddScoped<IAnalyticsUnitOfWork, AnalyticsUnitOfWork>();
builder.Services.AddScoped<AnalyticsAppService>();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<AnalyticsConsumerWorker>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HireConnect - Analytics Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, [] }
    });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("AnalyticsDb")!);

builder.Services.AddCors(o =>
    o.AddPolicy("AllowGateway", p =>
        p.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["*"])
         .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Analytics Service v1"));
}

app.UseSerilogRequestLogging();
app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();
