// Dans : DTO/CommentaireCreateModel.cs
using System.ComponentModel.DataAnnotations;

namespace ReclamationsAPI.DTO
{
    public class CommentaireCreateModel
    {
        [Required]
        public string Contenu { get; set; }

        public bool EstPrive { get; set; }
    }
}