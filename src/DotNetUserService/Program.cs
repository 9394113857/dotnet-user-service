using DotNetUserService.Data;
using DotNetUserService.Repositories;
using DotNetUserService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Database
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "DefaultConnection is not configured.");
    }

    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service
builder.Services.AddScoped<IUserService, UserService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// OpenAPI only during development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// HTTPS
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowFrontend");

// REST controllers
app.MapControllers();

app.Run();
