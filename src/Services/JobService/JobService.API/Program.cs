using BuildingBlocks.Common.Middleware;
using BuildingBlocks.Security.Extensions;
using BuildingBlocks.Security.Jwt;
using JobService.Application.Interfaces;
using JobService.Application.Services;
using JobService.Domain.Repositories;
using JobService.Infrastructure.Data;
using JobService.Infrastructure.Repositories;
using JobService.Infrastructure.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseNpgsql(
        BuildingBlocks.Common.Extensions.ConnectionStringParser.ParseUrlToNpgsql(builder.Configuration.GetConnectionString("JobDb")),
        npgsql => npgsql.MigrationsAssembly("JobService.Infrastructure")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConn = builder.Configuration.GetConnectionString("Redis");
    if (redisConn != null && redisConn.StartsWith("redis://"))
    {
        redisConn = redisConn.Substring(8);
    }
    options.Configuration = redisConn;
    options.InstanceName = "JobService_";
});

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobCategoryRepository, JobCategoryRepository>();
builder.Services.AddScoped<IJobUnitOfWork, JobUnitOfWork>();
builder.Services.AddScoped<JobApplicationService>();
builder.Services.AddScoped<IJobSearchService, ElasticsearchJobSearchService>();
builder.Services.AddScoped<IJobEventPublisher, JobEventPublisher>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HireConnect – Job Service", Version = "v1" });
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
    .AddNpgSql(BuildingBlocks.Common.Extensions.ConnectionStringParser.ParseUrlToNpgsql(builder.Configuration.GetConnectionString("JobDb"))!);

builder.Services.AddCors(o =>
    o.AddPolicy("AllowGateway", p =>
        p.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["*"])
         .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Service v1"));
}

app.UseSerilogRequestLogging();
app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JobDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();
