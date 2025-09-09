// Fichier : Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using ReclamationsAPI.Models;

namespace ReclamationsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Reclamation> Reclamations { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<SousCategorie> SousCategories { get; set; }
        public DbSet<Statut> Statuts { get; set; }
        public DbSet<ObservateurReclamation> ObservateursReclamations { get; set; }

        // --- NOUVELLE DÉCLARATION DE TABLE ---
        public DbSet<HistoriqueStatut> HistoriquesStatuts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuration pour ObservateurReclamation (existante) ---
            modelBuilder.Entity<ObservateurReclamation>()
                .HasKey(or => new { or.ReclamationId, or.ObservateurId });

            modelBuilder.Entity<ObservateurReclamation>()
                .HasOne(or => or.Reclamation)
                .WithMany(r => r.Observateurs)
                .HasForeignKey(or => or.ReclamationId);

            modelBuilder.Entity<ObservateurReclamation>()
                .HasOne(or => or.Observateur)
                .WithMany()
                .HasForeignKey(or => or.ObservateurId);

            // Cette configuration est importante pour éviter les problèmes de suppression en cascade
            // si un statut est supprimé, ce qui est peu probable mais une bonne pratique.
            modelBuilder.Entity<HistoriqueStatut>()
                .HasOne(h => h.AncienStatut)
                .WithMany()
                .HasForeignKey(h => h.AncienStatutId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression en cascade

            modelBuilder.Entity<HistoriqueStatut>()
                .HasOne(h => h.NouveauStatut)
                .WithMany()
                .HasForeignKey(h => h.NouveauStatutId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression en cascade
        }
    }
}