using BuildingBlocks.Common.Middleware;
using BuildingBlocks.Security.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using InterviewService.Application.Interfaces;
using InterviewService.Application.Services;
using InterviewService.Application.Validation;
using InterviewService.Domain.Repositories;
using InterviewService.Infrastructure.Clients;
using InterviewService.Infrastructure.Data;
using InterviewService.Infrastructure.Repositories;
using InterviewService.Infrastructure.Services;
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

builder.Services.AddDbContext<InterviewDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("InterviewDb"),
        npgsql => npgsql.MigrationsAssembly("InterviewService.Infrastructure")));

builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IInterviewUnitOfWork, InterviewUnitOfWork>();
builder.Services.AddScoped<InterviewApplicationService>();

builder.Services.AddHttpClient<IApplicationServiceClient, ApplicationServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("Services")["ApplicationServiceBaseUrl"] ?? "http://application-service:8080/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HireConnect - Interview Service", Version = "v1" });
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
    .AddNpgSql(builder.Configuration.GetConnectionString("InterviewDb")!);

builder.Services.AddCors(o =>
    o.AddPolicy("AllowGateway", p =>
        p.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["*"])
         .AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ScheduleInterviewRequestValidator>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interview Service v1"));
}

app.UseSerilogRequestLogging();
app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InterviewDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();
