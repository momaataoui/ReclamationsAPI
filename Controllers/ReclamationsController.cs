// Fichier : Controllers/ReclamationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReclamationsAPI.Data;
using ReclamationsAPI.DTO;    
using ReclamationsAPI.Models; 
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReclamationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sécurise toutes les actions du contrôleur par défaut
    public class ReclamationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReclamationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reclamations
        // Renvoie toutes les réclamations pour un Admin, ou seulement celles de l'utilisateur pour un Collaborateur.
        [HttpGet]
        public async Task<IActionResult> GetReclamations()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var query = _context.Reclamations
                .Include(r => r.Statut)
                .Include(r => r.Createur)
                .AsQueryable();

            if (userRole != RoleUtilisateur.Admin.ToString())
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                query = query.Where(r => r.UtilisateurId == userId);
            }

            var reclamations = await query
                .OrderByDescending(r => r.DateSoumission)
                .Select(r => new {
                    r.Id,
                    r.Objet,
                    r.DateSoumission,
                    Statut = r.Statut.Nom,
                    Auteur = new { r.Createur.Id, r.Createur.Nom }
                })
                .ToListAsync();

            return Ok(reclamations);
        }


        // GET: api/reclamations/{id}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReclamationById(int id)
        {
            var reclamation = await _context.Reclamations
                .Include(r => r.Statut)
                .Include(r => r.SousCategorie.Categorie)
                .Include(r => r.Createur)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reclamation == null) return NotFound();
            // ... (votre logique de sécurité reste la même)

            // --- TRANSFORMATION EN DTO ---
            var reclamationDto = new ReclamationDetailDto
            {
                Id = reclamation.Id,
                Objet = reclamation.Objet,
                Message = reclamation.Message,
                DateSoumission = reclamation.DateSoumission,
                Statut = new StatutDto { Id = reclamation.Statut.Id, Nom = reclamation.Statut.Nom },
                SousCategorie = new SousCategorieDto { /* ... mapper les champs ... */ },
                Createur = new AuteurDto { Id = reclamation.Createur.Id, Nom = reclamation.Createur.Nom }
            };

            return Ok(reclamationDto);
        }
        // POST: api/reclamations
        // Crée une nouvelle réclamation.
        [HttpPost]
        public async Task<IActionResult> CreateReclamation([FromBody] ReclamationCreateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var sousCategorieExiste = await _context.SousCategories.AnyAsync(sc => sc.Id == model.SousCategorieId);
            if (!sousCategorieExiste) return BadRequest(new { message = "La sous-catégorie sélectionnée n'existe pas." });

            var statutInitial = await _context.Statuts.FirstOrDefaultAsync(s => s.Nom.ToLower() == "en attente");
            if (statutInitial == null) return StatusCode(500, new { message = "Le statut par défaut 'En attente' est manquant." });

            var reclamation = new Reclamation
            {
                Objet = model.Objet,
                Message = model.Message,
                SousCategorieId = model.SousCategorieId,
                UtilisateurId = userId,
                DateSoumission = DateTime.UtcNow,
                StatutId = statutInitial.Id
            };

            _context.Reclamations.Add(reclamation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReclamationById), new { id = reclamation.Id }, reclamation);
        }

        // DELETE: api/reclamations/{id}
        // Un admin peut tout supprimer, un collaborateur a des restrictions.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReclamation(int id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var reclamationToDelete = await _context.Reclamations.Include(r => r.Statut).FirstOrDefaultAsync(r => r.Id == id);

            if (reclamationToDelete == null) return NotFound();

            if (userRole != RoleUtilisateur.Admin.ToString())
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (reclamationToDelete.UtilisateurId != userId) return Forbid();
                if (reclamationToDelete.Statut.Nom.ToLower() != "en attente") return BadRequest(new { message = "Vous ne pouvez supprimer une réclamation que si elle est 'En attente'." });
            }

            _context.Reclamations.Remove(reclamationToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/reclamations/{id}/statut
        // Endpoint réservé à l'admin pour répondre (changer le statut).



        [HttpPut("{id}/statut")]
        [Authorize(Roles = "Admin")] // Admin ou Assigné peuvent changer le statut
        public async Task<IActionResult> UpdateStatut(int id, [FromBody] UpdateStatutDto model)
        {
            var reclamation = await _context.Reclamations.FindAsync(id);
            if (reclamation == null) return NotFound();

            // --- LOGIQUE D'HISTORIQUE AJOUTÉE ICI ---
            var ancienStatutId = reclamation.StatutId;
            var nouveauStatutId = model.StatutId;

            // On ne fait rien si le statut n'a pas changé
            if (ancienStatutId == nouveauStatutId)
            {
                return Ok(new { message = "Le statut est déjà à jour." });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var historique = new HistoriqueStatut
            {
                ReclamationId = id,
                UtilisateurId = userId,
                AncienStatutId = ancienStatutId,
                NouveauStatutId = nouveauStatutId,
                DateChangement = DateTime.UtcNow
            };
            _context.HistoriquesStatuts.Add(historique);
            // --- FIN DE LA LOGIQUE D'HISTORIQUE ---

            reclamation.StatutId = nouveauStatutId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Statut mis à jour avec succès." });
        }

        // GET: api/reclamations/{id}/historique-statuts
        [HttpGet("{id}/historique-statuts")]
        public async Task<IActionResult> GetHistoriqueStatuts(int id)
        {

            var historique = await _context.HistoriquesStatuts
                .Where(h => h.ReclamationId == id)
                .Include(h => h.ModifiePar)
                .Include(h => h.AncienStatut)
                .Include(h => h.NouveauStatut)
                .OrderByDescending(h => h.DateChangement)
                .Select(h => new // On retourne un DTO propre
                {
                    h.DateChangement,
                    ModifiePar = h.ModifiePar.Nom,
                    AncienStatut = h.AncienStatut.Nom,
                    NouveauStatut = h.NouveauStatut.Nom
                })
                .ToListAsync();

            return Ok(historique);
        }
        // POST: api/reclamations/{id}/commentaires
        // Permet à l'admin de répondre et d'ajouter des notes privées.

        [HttpPost("{id}/commentaires")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CommentaireCreateModel model)
        {
            // ... (votre logique de vérification et de création est bonne)
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Utilisateurs.FindAsync(userId); // On récupère l'utilisateur pour son nom
            if (user == null) return Unauthorized();

            var commentaire = new Commentaire
            {
                Contenu = model.Contenu,
                EstPrive = model.EstPrive,
                ReclamationId = id,
                UtilisateurId = userId
            };

            _context.Commentaires.Add(commentaire);
            await _context.SaveChangesAsync();

            // --- TRANSFORMATION EN DTO AVANT DE RETOURNER ---
            var commentaireDto = new CommentaireDto
            {
                Id = commentaire.Id,
                Contenu = commentaire.Contenu,
                DateCreation = commentaire.DateCreation,
                EstPrive = commentaire.EstPrive,
                UtilisateurId = commentaire.UtilisateurId,
                AuteurNom = user.Nom, // On ajoute le nom
                ReclamationId = commentaire.ReclamationId
            };

            return Ok(commentaireDto); // On retourne le DTO, pas l'entité complète
        }
        // GET: api/reclamations/{id}/commentaires
        // Dans : Controllers/ReclamationsController.cs

        // GET: api/reclamations/{id}/commentaires
        [HttpGet("{id}/commentaires")]
        public async Task<IActionResult> GetComments(int id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var reclamation = await _context.Reclamations.FindAsync(id);
            if (reclamation == null) return NotFound();

            if (userRole != RoleUtilisateur.Admin.ToString())
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (reclamation.UtilisateurId != userId) return Forbid();
            }

            IQueryable<Commentaire> query = _context.Commentaires
                .Include(c => c.Auteur)
                .Where(c => c.ReclamationId == id);

            if (userRole == RoleUtilisateur.Collaborateur.ToString())
            {
                query = query.Where(c => !c.EstPrive);
            }

            var comments = await query
                .OrderBy(c => c.DateCreation)
                .Select(c => new
                {
                    Id = c.Id,                   // On ajoute l'ID du commentaire
                    c.Contenu,
                    c.DateCreation,
                    c.EstPrive,
                    Auteur = c.Auteur.Nom,
                    AuteurId = c.UtilisateurId   // On ajoute l'ID de l'auteur du commentaire
                })
                .ToListAsync();

            return Ok(comments);
        }


        // DELETE: api/reclamations/commentaires/{commentaireId}
        [HttpDelete("commentaires/{commentaireId}")]
        public async Task<IActionResult> DeleteComment(int commentaireId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var commentaireToDelete = await _context.Commentaires
                .Include(c => c.Reclamation) // On inclut la réclamation pour vérifier son statut
                .FirstOrDefaultAsync(c => c.Id == commentaireId);

            // Règle 1 : Le commentaire doit exister
            if (commentaireToDelete == null)
            {
                return NotFound("Commentaire non trouvé.");
            }

            bool isAdmin = userRole == RoleUtilisateur.Admin.ToString();

            // Si l'utilisateur est un Admin, il peut tout supprimer.
            // Si ce n'est PAS un Admin, on applique les règles pour les collaborateurs.
            if (!isAdmin)
            {
                // Règle 2 : Le collaborateur doit être l'auteur du commentaire
                if (commentaireToDelete.UtilisateurId != userId)
                {
                    return Forbid(); // Non autorisé à supprimer le commentaire d'un autre
                }

                // Règle 3 : Le collaborateur ne peut supprimer que si la réclamation est "En attente"
                var reclamationStatut = await _context.Statuts.FindAsync(commentaireToDelete.Reclamation.StatutId);
                if (reclamationStatut?.Nom.ToLower() != "en attente")
                {
                    return BadRequest(new { message = "Vous ne pouvez plus supprimer ce commentaire car la réclamation est en cours de traitement." });
                }
            }

            // Si toutes les règles sont respectées, on supprime
            _context.Commentaires.Remove(commentaireToDelete);
            await _context.SaveChangesAsync();

            return NoContent(); // Succès (HTTP 204)
        }
    }
}