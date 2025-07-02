using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSBInvestmentPredictor.Expenses.Infrastructure.Data;

namespace WSBInvestmentPredictor.Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpensesDbContext _context;

    public ExpensesController(ExpensesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Runs database migrations for the Expenses module.
    /// This endpoint is used by CI/CD pipelines to ensure the database is up to date.
    /// </summary>
    /// <returns>Status of the migration operation</returns>
    [HttpPost("migrate")]
    public async Task<IActionResult> Migrate()
    {
        try
        {
            await _context.Database.MigrateAsync();
            
            return Ok(new
            {
                success = true,
                message = "Database migrations completed successfully",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Database migration failed",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Gets the current status of the Expenses module database.
    /// </summary>
    /// <returns>Database status information</returns>
    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            
            return Ok(new
            {
                success = true,
                databaseName = _context.Database.GetDbConnection().Database,
                pendingMigrations = pendingMigrations.ToArray(),
                appliedMigrations = appliedMigrations.ToArray(),
                isConnected = await _context.Database.CanConnectAsync(),
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to get database status",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
} 