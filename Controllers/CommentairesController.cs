namespace ReclamationsAPI.Controllers
{
    // Fichier : Controllers/CommentairesController.cs

    using global::ReclamationsAPI.Data;
    using global::ReclamationsAPI.DTO.ReclamationsAPI.DTO;
    using global::ReclamationsAPI.Models;
    // --- ETAPE 1 : AJOUT DE TOUS LES 'USING' NÉCESSAIRES ---
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic; // Nécessaire pour List<T>
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace ReclamationsAPI.Controllers
    {
        [Route("api/reclamations/{reclamationId}/commentaires")]
        [ApiController]
        [Authorize]
        public class CommentairesController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public CommentairesController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: api/reclamations/123/commentaires
            // Récupère les commentaires pour une réclamation, en filtrant selon le rôle.
            [HttpGet]
            public async Task<ActionResult<IEnumerable<CommentaireViewModel>>> GetCommentaires(int reclamationId)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null) return Unauthorized();
                var userId = int.Parse(userIdString);

                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var reclamation = await _context.Reclamations.FindAsync(reclamationId);
                if (reclamation == null) return NotFound("Réclamation non trouvée.");

                // Règle de sécurité : Qui peut voir les commentaires ?
                bool isAllowedToView = reclamation.UtilisateurId == userId ||
                                       reclamation.AssigneAId == userId ||
                                       userRole == RoleUtilisateur.SuperAdmin.ToString();

                if (!isAllowedToView) return Forbid();

                var query = _context.Commentaires
                    .Where(c => c.ReclamationId == reclamationId)
                    .Include(c => c.Auteur)
                    .OrderBy(c => c.DateCreation);

                // RÈGLE MÉTIER : On masque les commentaires privés pour le Collaborateur.
                IQueryable<Commentaire> finalQuery = query;
                if (userRole == RoleUtilisateur.Collaborateur.ToString())
                {
                    finalQuery = query.Where(c => !c.EstPrive);
                }

                var commentaires = await finalQuery
                    .Select(c => new CommentaireViewModel
                    {
                        Id = c.Id,
                        Contenu = c.Contenu,
                        DateCreation = c.DateCreation,
                        EstPrive = c.EstPrive,
                        AuteurNom = c.Auteur.Nom,
                        AuteurRole = c.Auteur.Role.ToString()
                    })
                    .ToListAsync();

                return Ok(commentaires);
            }

            // POST: api/reclamations/123/commentaires
            // Ajoute un commentaire à une réclamation en respectant les permissions.
            [HttpPost]
            public async Task<ActionResult<CommentaireViewModel>> AddCommentaire(int reclamationId, [FromBody] CommentaireCreateModel model)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null) return Unauthorized();
                var userId = int.Parse(userIdString);

                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var reclamation = await _context.Reclamations.FindAsync(reclamationId);
                if (reclamation == null) return NotFound("Réclamation non trouvée.");

                // RÈGLE MÉTIER : Un Collaborateur ne peut pas poster de commentaire privé.
                if (model.EstPrive && userRole == RoleUtilisateur.Collaborateur.ToString())
                {
                    return Forbid("Vous n'êtes pas autorisé à poster un commentaire privé.");
                }

                var commentaire = new Commentaire
                {
                    Contenu = model.Contenu,
                    EstPrive = model.EstPrive,
                    ReclamationId = reclamationId,
                    UtilisateurId = userId
                    // DateCreation est générée automatiquement par le modèle
                };

                _context.Commentaires.Add(commentaire);
                await _context.SaveChangesAsync();

                // On récupère le nom de l'auteur pour le renvoyer dans la réponse.
                var auteur = await _context.Utilisateurs.FindAsync(userId);

                var commentaireViewModel = new CommentaireViewModel
                {
                    Id = commentaire.Id,
                    Contenu = commentaire.Contenu,
                    DateCreation = commentaire.DateCreation,
                    EstPrive = commentaire.EstPrive,
                    AuteurNom = auteur.Nom,
                    AuteurRole = auteur.Role.ToString()
                };

                // On retourne une réponse 201 Created avec le commentaire créé.
                return CreatedAtAction(nameof(GetCommentaires), new { reclamationId = reclamationId }, commentaireViewModel);
            }
        }
    }
}
