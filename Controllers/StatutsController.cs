using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReclamationsAPI.Data;

namespace ReclamationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Seuls les admins ont besoin de cette liste
    public class StatutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/statuts
        [HttpGet]
        public async Task<IActionResult> GetStatuts()
        {
            return Ok(await _context.Statuts.ToListAsync());
        }
    }
}