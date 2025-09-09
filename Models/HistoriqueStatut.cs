// Dans : Models/HistoriqueStatut.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReclamationsAPI.Models
{
    public class HistoriqueStatut
    {
        public int Id { get; set; }
        public DateTime DateChangement { get; set; } = DateTime.UtcNow;

        // Qui a fait le changement ?
        public int UtilisateurId { get; set; }
        public Utilisateur ModifiePar { get; set; }

        // Quelle réclamation est concernée ?
        public int ReclamationId { get; set; }
        public Reclamation Reclamation { get; set; }

        // Quel était l'ancien statut et quel est le nouveau ?
        public int AncienStatutId { get; set; }
        [ForeignKey("AncienStatutId")]
        public Statut AncienStatut { get; set; }

        public int NouveauStatutId { get; set; }
        [ForeignKey("NouveauStatutId")]
        public Statut NouveauStatut { get; set; }
    }
}