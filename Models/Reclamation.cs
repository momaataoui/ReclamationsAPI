// Dans : Models/Reclamation.cs



namespace ReclamationsAPI.Models
{
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
    public class Reclamation
    {
        public int Id { get; set; }

        // --- MODIFICATION ---
        // Renommé "Titre" en "Objet" et "Description" en "Message" pour correspondre à l'UI.
        public string Objet { get; set; }
        public string Message { get; set; }

        // Renommé "DateCreation" pour plus de clarté
        public DateTime DateSoumission { get; set; } = DateTime.UtcNow;

        // --- RELATIONS (INCHANGÉES) ---
        public int UtilisateurId { get; set; }
        [ForeignKey("UtilisateurId")]
        public Utilisateur Createur { get; set; }

        public int StatutId { get; set; }
        public Statut Statut { get; set; }

        public int SousCategorieId { get; set; }
        public SousCategorie SousCategorie { get; set; }

        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();

        // --- NOUVELLE PROPRIÉTÉ POUR L'ASSIGNATION ---
        // L'ID de l'utilisateur assigné. Le '?' signifie qu'il peut être nul (non assigné).
        public int? AssigneAId { get; set; }

        // Propriété de navigation vers l'utilisateur assigné
        [ForeignKey("AssigneAId")]
        public Utilisateur AssigneA { get; set; }


        public ICollection<ObservateurReclamation> Observateurs { get; set; } = new List<ObservateurReclamation>();
    }
}