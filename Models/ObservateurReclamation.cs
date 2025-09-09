// Fichier : Models/ObservateurReclamation.cs

namespace ReclamationsAPI.Models
{
    // Cette classe sert de table de liaison entre une Réclamation et les Utilisateurs qui l'observent.
    public class ObservateurReclamation
    {
        public int ReclamationId { get; set; }
        public Reclamation Reclamation { get; set; }

        public int ObservateurId { get; set; }
        public Utilisateur Observateur { get; set; }
    }
}