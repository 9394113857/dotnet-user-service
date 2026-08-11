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

// Root status page
app.MapGet("/", () => Results.Content("""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>DotNet User Service</title>

        <style>
            * {
                box-sizing: border-box;
            }

            body {
                margin: 0;
                min-height: 100vh;
                display: flex;
                align-items: center;
                justify-content: center;
                font-family: Arial, Helvetica, sans-serif;
                background: linear-gradient(135deg, #f0f4f8, #dbeafe);
                color: #1f2937;
            }

            .card {
                width: 90%;
                max-width: 520px;
                background: #ffffff;
                padding: 40px;
                border-radius: 18px;
                box-shadow: 0 15px 40px rgba(0, 0, 0, 0.10);
            }

            h1 {
                margin: 0 0 10px;
                font-size: 30px;
                color: #111827;
            }

            .subtitle {
                margin-bottom: 30px;
                color: #6b7280;
            }

            .status {
                display: inline-flex;
                align-items: center;
                gap: 8px;
                padding: 8px 14px;
                border-radius: 999px;
                background: #dcfce7;
                color: #15803d;
                font-weight: bold;
                margin-bottom: 25px;
            }

            .dot {
                width: 9px;
                height: 9px;
                background: #22c55e;
                border-radius: 50%;
            }

            .section {
                margin-top: 25px;
            }

            .section h2 {
                font-size: 18px;
                margin-bottom: 12px;
            }

            .endpoint {
                display: block;
                padding: 12px 14px;
                margin: 8px 0;
                background: #f3f4f6;
                border-radius: 8px;
                color: #374151;
                text-decoration: none;
                font-family: monospace;
            }

            .endpoint:hover {
                background: #e5e7eb;
            }

            .footer {
                margin-top: 30px;
                font-size: 13px;
                color: #9ca3af;
            }
        </style>
    </head>

    <body>
        <div class="card">
            <h1>DotNet User Service</h1>

            <p class="subtitle">
                ASP.NET Core API running on Render
            </p>

            <div class="status">
                <span class="dot"></span>
                Service Online
            </div>

            <div class="section">
                <h2>API Status</h2>

                <a class="endpoint" href="/health">
                    GET /health
                </a>

                <a class="endpoint" href="/api/users">
                    GET /api/users
                </a>
            </div>

            <div class="footer">
                DotNet User Service • ASP.NET Core .NET 9
            </div>
        </div>
    </body>
    </html>
    """, "text/html"));

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok"
}));

// REST controllers
app.MapControllers();

app.Run();