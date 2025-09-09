namespace ReclamationsAPI.DTO
{
    // Dans : DTO/CommentaireDtos.cs

    using System.ComponentModel.DataAnnotations;

    namespace ReclamationsAPI.DTO
    {
        // DTO pour la création d'un commentaire
        public class CommentaireCreateModel
        {
            [Required(ErrorMessage = "Le contenu du commentaire est obligatoire.")]
            public string Contenu { get; set; }

            // Le frontend enverra 'true' si le commentaire doit être privé.
            public bool EstPrive { get; set; } = false;
        }

        // DTO pour l'affichage d'un commentaire
        public class CommentaireViewModel
        {
            public int Id { get; set; }
            public string Contenu { get; set; }
            public DateTime DateCreation { get; set; }
            public bool EstPrive { get; set; }
            public string AuteurNom { get; set; }
            public string AuteurRole { get; set; }
        }
    }
}
