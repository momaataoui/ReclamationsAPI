// Dans Controllers/DashboardController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReclamationsAPI.Data;
using ReclamationsAPI.DTO;
using ReclamationsAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // Cet endpoint est réservé aux admins
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/dashboard/stats
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var allReclamations = await _context.Reclamations.Include(r => r.Statut).ToListAsync();
        var today = DateTime.UtcNow.Date;
        var sevenDaysAgo = today.AddDays(-6);

        var stats = new DashboardStatsDto
        {
            TotalReclamations = allReclamations.Count,
            EnCours = allReclamations.Count(r => r.Statut.Nom.ToLower().Contains("en cours")),
            Resolues = allReclamations.Count(r => r.Statut.Nom.ToLower().Contains("résolu") || r.Statut.Nom.ToLower().Contains("fermé")),

            RepartitionParStatut = allReclamations
                .GroupBy(r => r.Statut.Nom)
                .Select(g => new StatSummary { StatutNom = g.Key, Count = g.Count() })
                .ToList(),

            Tendances7Jours = allReclamations
                .Where(r => r.DateSoumission.Date >= sevenDaysAgo && r.DateSoumission.Date <= today)
                .GroupBy(r => r.DateSoumission.Date)
                .Select(g => new DailyTrend { Date = g.Key, Count = g.Count() })
                .OrderBy(d => d.Date)
                .ToList()
        };

        return Ok(stats);
    }
}