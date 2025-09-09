using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReclamationsAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SousCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    CategorieId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SousCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SousCategories_Categories_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reclamations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Objet = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatutId = table.Column<int>(type: "INTEGER", nullable: false),
                    SousCategorieId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssigneAId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reclamations_SousCategories_SousCategorieId",
                        column: x => x.SousCategorieId,
                        principalTable: "SousCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reclamations_Statuts_StatutId",
                        column: x => x.StatutId,
                        principalTable: "Statuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reclamations_Utilisateurs_AssigneAId",
                        column: x => x.AssigneAId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reclamations_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commentaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Contenu = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstPrive = table.Column<bool>(type: "INTEGER", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReclamationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commentaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commentaires_Reclamations_ReclamationId",
                        column: x => x.ReclamationId,
                        principalTable: "Reclamations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commentaires_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObservateursReclamations",
                columns: table => new
                {
                    ReclamationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObservateurId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservateursReclamations", x => new { x.ReclamationId, x.ObservateurId });
                    table.ForeignKey(
                        name: "FK_ObservateursReclamations_Reclamations_ReclamationId",
                        column: x => x.ReclamationId,
                        principalTable: "Reclamations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObservateursReclamations_Utilisateurs_ObservateurId",
                        column: x => x.ObservateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commentaires_ReclamationId",
                table: "Commentaires",
                column: "ReclamationId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaires_UtilisateurId",
                table: "Commentaires",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservateursReclamations_ObservateurId",
                table: "ObservateursReclamations",
                column: "ObservateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_AssigneAId",
                table: "Reclamations",
                column: "AssigneAId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_SousCategorieId",
                table: "Reclamations",
                column: "SousCategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_StatutId",
                table: "Reclamations",
                column: "StatutId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_UtilisateurId",
                table: "Reclamations",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_SousCategories_CategorieId",
                table: "SousCategories",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commentaires");

            migrationBuilder.DropTable(
                name: "ObservateursReclamations");

            migrationBuilder.DropTable(
                name: "Reclamations");

            migrationBuilder.DropTable(
                name: "SousCategories");

            migrationBuilder.DropTable(
                name: "Statuts");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
