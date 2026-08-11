using DotNetUserService.Data;
using DotNetUserService.Repositories;
using DotNetUserService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Database provider
// Local: SQLite
// Render: PostgreSQL
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
    {
        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "PostgreSQL connection string 'DefaultConnection' was not found.");

        options.UseNpgsql(connectionString);
    }
    else
    {
        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=app.db";

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

// Render provides the PORT environment variable.
// Local/default port is 10000.
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Apply database initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
    {
        // Production: apply PostgreSQL EF Core migrations.
        dbContext.Database.Migrate();
    }
    else
    {
        // Local development: create SQLite database automatically.
        dbContext.Database.EnsureCreated();
    }
}

// OpenAPI only during development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // HTTPS redirection only for local development.
    app.UseHttpsRedirection();
}

// CORS
app.UseCors("AllowFrontend");

// REST controllers
app.MapControllers();

app.Run();
