namespace ReclamationsAPI.Models
{
public class Categorie
{
    public int Id { get; set; } 
    public string Nom { get; set; } = string.Empty;
    public ICollection<SousCategorie> SousCategories { get; set; } = new List<SousCategorie>();
    }

}
