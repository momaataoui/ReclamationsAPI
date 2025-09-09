namespace ReclamationsAPI.Models
{
public class SousCategorie
{
    public int Id { get; set; }
    public string Nom { get; set; }

    public int CategorieId { get; set; }
    public Categorie Categorie { get; set; }
}

}

