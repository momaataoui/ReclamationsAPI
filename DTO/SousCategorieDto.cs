namespace ReclamationsAPI.DTO
{
    public class SousCategorieDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public CategorieDto Categorie { get; set; } // On inclut le DTO parent
    }
}