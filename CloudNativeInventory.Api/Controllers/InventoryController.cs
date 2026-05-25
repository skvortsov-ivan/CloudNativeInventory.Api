using CloudNativeInventory.Api.Data;
using CloudNativeInventory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudNativeInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(InventoryDbContext context, IConfiguration configuration, ILogger<InventoryController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("list/products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    // Denna endpoint används för att bevisa att appen framgångsrikt har hämtat den hemliga integrationsnyckeln (från Azure Key Vault i prod)
    [HttpGet("system/verify-integration")]
    public IActionResult VerifyExternalIntegration()
    {
        var apiKey = _configuration["ExternalServices:VendorApiKey"];

        if (string.IsNullOrEmpty(apiKey) || apiKey == "LOCAL_DEV_SECRET_12345_DO_NOT_DEPLOY")
        {
            return StatusCode(500, new { Status = "Unsecured", Message = "Körs med lokal (eller saknad) hemlighet!" });
        }

        return Ok(new { Status = "Secured", Message = "Hemlighet laddades framgångsrikt via säker konfiguration." });
    }

    [HttpGet("health")]
    public IActionResult GetHealth([FromQuery] bool simulateCrash = false)
    {
        _logger.LogInformation("Health-check anropades. Applikationen är vaken.");

        if (simulateCrash)
        {
            _logger.LogError("Ett simulerat fel framkallades via health-endpointen!");
            throw new Exception("Kritiskt fel: Simulerad krasch för Application Insights!");
        }

        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        _logger.LogInformation("Försöker hämta API-nyckel från konfigurationen...");
        var secretValue = _configuration["ExternalServices:VendorApiKey"];

        if (string.IsNullOrEmpty(secretValue))
        {
            _logger.LogWarning("Hemligheten 'ExternalServices:VendorApiKey' kunde inte hittas eller är tom!");
            return NotFound("Ingen hemlighet hittades. Kontrollera Key Vault och Managed Identity-rättigheter.");
        }

        return Ok(new { SecretMessage = secretValue, Message = "Hemlighet hämtades framgångsrikt från Key Vault!" });
    }
}

