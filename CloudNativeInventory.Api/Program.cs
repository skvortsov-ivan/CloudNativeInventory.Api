using Azure.Identity; // TODO (Del 4): Krävs för Key Vault
using Azure.Monitor.OpenTelemetry.AspNetCore;
using CloudNativeInventory.Api.Data;
using CloudNativeInventory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

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

//I need to add a new controller to my project. In my previous one I have this endpoint "    [HttpGet("system/verify-integration")]
//public IActionResult VerifyExternalIntegration()
//{
//    var apiKey = _configuration["ExternalServices:VendorApiKey"];

//    if (string.IsNullOrEmpty(apiKey) || apiKey == "LOCAL_DEV_SECRET_12345_DO_NOT_DEPLOY")
//    {
//        return StatusCode(500, new { Status = "Unsecured", Message = "Körs med lokal (eller saknad) hemlighet!" });
//    }

//    return Ok(new { Status = "Secured", Message = "Hemlighet laddades framgångsrikt via säker konfiguration." });
//}
//". and in my appsettings.json I have this: "{
//    "Logging": {
//        "LogLevel": {
//            "Default": "Information",
//      "Microsoft.AspNetCore": "Warning"
//        }
//    },
//  "AllowedHosts": "*",
//  "KeyVaultUrl": "https://kvstudentjtoxigkxpzska.vault.azure.net/",
//  "ExternalServices": {
//        "VendorApiKey": ""
//  }
//}
//"