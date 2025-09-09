

    // Code à coller dans DTO/ReclamationCreateModel.cs
using System.ComponentModel.DataAnnotations;
namespace ReclamationsAPI.DTO
{
    public class ReclamationCreateModel
    {
        [Required]
        public string Objet { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public int SousCategorieId { get; set; }
    }
}

