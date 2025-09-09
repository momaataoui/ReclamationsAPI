namespace ReclamationsAPI.Controllers
{
    // Fichier: Controllers/CategoriesController.cs
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ReclamationsAPI.Data;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        // Renvoie la liste de toutes les catégories principales (Thèmes)
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new { c.Id, c.Nom }) // On ne renvoie que l'ID et le Nom
                .ToListAsync();
            return Ok(categories);
        }

        // GET: api/Categories/5/SousCategories
        // Renvoie les sous-catégories pour une catégorie donnée
        [HttpGet("{id}/SousCategories")]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            var subCategories = await _context.SousCategories
                .Where(sc => sc.CategorieId == id)
                .Select(sc => new { sc.Id, sc.Nom })
                .ToListAsync();
            return Ok(subCategories);
        }
    }
}
