// Dans : DTO/CommentaireDto.cs
namespace ReclamationsAPI.DTO
{
    public class CommentaireDto
    {
        public int Id { get; set; }
        public string Contenu { get; set; }
        public DateTime DateCreation { get; set; }
        public bool EstPrive { get; set; }
        public int UtilisateurId { get; set; }
        public string AuteurNom { get; set; } // On ajoute le nom de l'auteur
        public int ReclamationId { get; set; }
    }
}