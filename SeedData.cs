// Fichier: SeedData.cs
using Microsoft.EntityFrameworkCore;
using ReclamationsAPI.Data;
using ReclamationsAPI.Models;
using System.Linq;

namespace ReclamationsAPI
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // S'assure que la base de données est créée.
            context.Database.EnsureCreated();

            Console.WriteLine("--- Checking for initial data to seed... ---");

            // --- 1. Seed Catégories & Sous-Catégories ---
            // Dans : Data/SeedData.cs

            // --- 1. Seed Catégories & Sous-Catégories (Version SQLI) ---
            if (!context.Categories.Any())
            {
                Console.WriteLine("Seeding Categories and Subcategories for SQLI...");
                var categories = new Categorie[]
                {
        // NOUVEAUX THÈMES PERTINENTS POUR SQLI
        new Categorie{Nom = "Support Technique & IT"},
        new Categorie{Nom = "Ressources Humaines (RH)"},
        new Categorie{Nom = "Services Généraux & Logistique"}
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();

                var sousCategories = new SousCategorie[]
                {
        // Sous-catégories pour Support IT
        new SousCategorie{Nom = "Problème Matériel (PC, Souris, Écran)", CategorieId = categories[0].Id},
        new SousCategorie{Nom = "Accès Logiciel ou Licence", CategorieId = categories[0].Id},
        new SousCategorie{Nom = "Problème de Connexion (Réseau/VPN)", CategorieId = categories[0].Id},
        
        // Sous-catégories pour RH
        new SousCategorie{Nom = "Question sur la Fiche de Paie", CategorieId = categories[1].Id},
        new SousCategorie{Nom = "Demande d'Attestation de Travail", CategorieId = categories[1].Id},
        new SousCategorie{Nom = "Information sur les Congés ou Absences", CategorieId = categories[1].Id},

        // Sous-catégories pour Services Généraux
        new SousCategorie{Nom = "Demande de Fournitures de Bureau", CategorieId = categories[2].Id},
        new SousCategorie{Nom = "Problème dans les Locaux (salle, climatisation...)", CategorieId = categories[2].Id},
                };
                context.SousCategories.AddRange(sousCategories);
                context.SaveChanges();
            }

            // --- 2. Seed Statuts ---
            if (!context.Statuts.Any())
            {
                Console.WriteLine("Seeding Statuts...");
                var statuts = new Statut[]
                {
                    new Statut{Nom = "En attente"},
                    new Statut{Nom = "En cours de traitement"},
                    new Statut{Nom = "Fermée"}
                };
                context.Statuts.AddRange(statuts);
                context.SaveChanges();
            }

            // --- 3. MODIFICATION PRINCIPALE : Seed de l'Utilisateur Admin Unique ---
            // On vérifie si un utilisateur avec l'email admin@gmail.com n'existe pas déjà.
            if (!context.Utilisateurs.Any(u => u.Email == "admin@gmail.com"))
            {
                Console.WriteLine("Seeding Admin User...");
                var adminUser = new Utilisateur
                {
                    Nom = "Administrateur",
                    Email = "admin@gmail.com", // L'email unique de l'admin
                    // Le mot de passe par défaut est "admin1234"
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin1234"),
                    Role = RoleUtilisateur.Admin // On lui assigne le rôle Admin
                };
                context.Utilisateurs.Add(adminUser);
                context.SaveChanges();
            }

            Console.WriteLine("--- Seeding process complete. ---");
        }
    }
}