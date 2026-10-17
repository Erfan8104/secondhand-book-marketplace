using BookMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    Environment.GetEnvironmentVariable("BOOKMARKETPLACE_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "BOOKMARKETPLACE_CONNECTION_STRING environment variable is missing.");
}

builder.Services.AddControllers();

builder.Services.AddDbContext<MarketplaceDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<MarketplaceDbContext>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "BookMarketplace.Api"
}));

app.MapHealthChecks("/health/database");
app.MapControllers();

app.Run();