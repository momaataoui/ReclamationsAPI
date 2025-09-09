// Fichier: Controllers/UtilisateursController.cs

using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReclamationsAPI.Data;
using ReclamationsAPI.DTO; 
using ReclamationsAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UtilisateursController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UtilisateursController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // --- ENDPOINT PUBLIC POUR L'INSCRIPTION ---
    // POST: api/utilisateurs/register
    [HttpPost("register")]
    [AllowAnonymous] // Permet à n'importe qui d'appeler cette méthode
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (await _context.Utilisateurs.AnyAsync(u => u.Email == model.Email))
        {
            return BadRequest("Cet email est déjà utilisé.");
        }

        var utilisateur = new Utilisateur
        {
            Nom = model.Nom,
            Email = model.Email,
            // On hache le mot de passe fourni
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.MotDePasse),
            // Par défaut, tout nouvel utilisateur est un Collaborateur
            Role = RoleUtilisateur.Collaborateur
        };

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();

        // On ne retourne jamais le mot de passe, même haché
        var userToReturn = new { utilisateur.Id, utilisateur.Nom, utilisateur.Email, utilisateur.Role };
        return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, userToReturn);
    }


    // --- ENDPOINT PUBLIC POUR LA CONNEXION ---
    // POST: api/utilisateurs/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == model.Email);

        // On vérifie si l'utilisateur existe ET si le mot de passe est correct
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.MotDePasse, user.PasswordHash))
        {
            return Unauthorized("Email ou mot de passe incorrect.");
        }

        // --- Génération du Token JWT ---
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()) // On inclut le rôle dans le token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(7); // Le token expirera dans 7 jours

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }


    // --- SECTION PROTÉGÉE POUR LES ADMINS ---

    // GET: api/utilisateurs
    // Seul un Admin peut voir la liste de tous les utilisateurs
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUtilisateurs()
    {
        // On sélectionne les champs à retourner pour ne jamais exposer le PasswordHash
        var users = await _context.Utilisateurs
            .Select(u => new { u.Id, u.Nom, u.Email, u.Role })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/utilisateurs/5
    // Seul un Admin peut voir le profil d'un utilisateur spécifique
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetUtilisateur(int id)
    {
        var utilisateur = await _context.Utilisateurs
            .Select(u => new { u.Id, u.Nom, u.Email, u.Role })
            .FirstOrDefaultAsync(u => u.Id == id);

        if (utilisateur == null)
        {
            return NotFound();
        }

        return Ok(utilisateur);
    }

    // Le PUT et le DELETE générés automatiquement sont dangereux car ils ne sont pas
    // assez sécurisés. Il est préférable de ne pas les inclure pour l'instant
    // ou de les réécrire complètement avec des règles de sécurité.
    // J'ai donc supprimé les méthodes PutUtilisateur et DeleteUtilisateur pour la sécurité.
}