using Azure.Identity; // TODO (Del 4): Krävs för Key Vault
using CloudNativeInventory.Api.Data;
using CloudNativeInventory.Api.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Aktiverar OpenTelemetry och skickar data automatiskt till Azure Monitor / Application Insights
builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddControllers();
builder.Services.AddOpenApi(); // .NET 9 OpenAPI

builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictSecurityPolicy", policyBuilder =>
    {
        //policyBuilder.WithOrigins("https://min-sakra-frontend-app.azurewebsites.net").WithMethods("GET", "POST").AllowAnyHeader();
        policyBuilder.WithOrigins("null");
    });
});

// 1. Läs in KeyVaultUrl från konfigurationen
var keyVaultUrl = builder.Configuration["KeyVaultUrl"];

// 2. Om KeyVaultUrl är satt, koppla in Azure Key Vault som config provider
//if (!string.IsNullOrEmpty(keyVaultUrl))
//{
//    builder.Configuration.AddAzureKeyVault(
//        new Uri(keyVaultUrl),
//        new DefaultAzureCredential());
//}

// TODO (Del 4 i "Tips och förslag"): Konfigurera Azure Key Vault
// Använd Managed Identity för att hämta hemligheter i produktion.

// Vi använder InMemory-databas lokalt
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseInMemoryDatabase("InventoryDb"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseCors("StrictSecurityPolicy");
}
app.UseAuthorization();
app.MapControllers();

// Seeda data (se till att vi inte dubblar om appen startas om i samma process)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    if (!db.Products.Any())
    {
        db.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 9999, StockQuantity = 10 });
        db.SaveChanges();
    }
}

// quick health root so / returns something
app.MapGet("/", () => Results.Ok("API is running"));

// debug endpoint that lists all registered endpoints
app.MapGet("/debug/routes", (EndpointDataSource ds) =>
{
    var routes = ds.Endpoints
        .Select(e => new { DisplayName = e.DisplayName, Pattern = (e as RouteEndpoint)?.RoutePattern?.RawText })
        .ToList();
    return Results.Ok(routes);
});

app.Run();

