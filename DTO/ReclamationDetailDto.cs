namespace ReclamationsAPI.DTO
{
    public class ReclamationDetailDto
    {
        public int Id { get; set; }
        public string Objet { get; set; }
        public string Message { get; set; }
        public DateTime DateSoumission { get; set; }
        // On utilise des DTOs imbriqués pour les relations
        public StatutDto Statut { get; set; }
        public SousCategorieDto SousCategorie { get; set; }
        public AuteurDto Createur { get; set; }
    }
    // N'oubliez pas de créer aussi les DTOs StatutDto, SousCategorieDto, CategorieDto, AuteurDto...
}