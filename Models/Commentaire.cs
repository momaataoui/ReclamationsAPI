// Fichier : Models/Commentaire.cs

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReclamationsAPI.Models
{
     public class Commentaire
     {
         public int Id { get; set; }

         // On utilise "Contenu" comme nom unique pour le texte du commentaire.
         public string Contenu { get; set; } = string.Empty;

         public DateTime DateCreation { get; set; } = DateTime.UtcNow;

         // On a ajouté cette propriété pour savoir si c'est une note privée.
         public bool EstPrive { get; set; }

         // --- RELATION AVEC L'AUTEUR (Utilisateur) ---
         // Le nom de la clé étrangère est UtilisateurId pour être cohérent.
         public int UtilisateurId { get; set; }
         public Utilisateur Auteur { get; set; }

         // --- RELATION AVEC LA RÉCLAMATION ---
         public int ReclamationId { get; set; }
         public Reclamation Reclamation { get; set; }

     
     }
 }