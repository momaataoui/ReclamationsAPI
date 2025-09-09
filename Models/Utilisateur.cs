// Dans : Models/Utilisateur.cs

namespace ReclamationsAPI.Models
{
    public enum RoleUtilisateur
    {
        Collaborateur, // Rôle par défaut
        Admin,        // Le super-utilisateur
    }

    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Par défaut, tout nouvel utilisateur est un Collaborateur.
        public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Collaborateur;
    }
}