using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReclamationsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoriqueStatutTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoriquesStatuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateChangement = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReclamationId = table.Column<int>(type: "INTEGER", nullable: false),
                    AncienStatutId = table.Column<int>(type: "INTEGER", nullable: false),
                    NouveauStatutId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriquesStatuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriquesStatuts_Reclamations_ReclamationId",
                        column: x => x.ReclamationId,
                        principalTable: "Reclamations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoriquesStatuts_Statuts_AncienStatutId",
                        column: x => x.AncienStatutId,
                        principalTable: "Statuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriquesStatuts_Statuts_NouveauStatutId",
                        column: x => x.NouveauStatutId,
                        principalTable: "Statuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoriquesStatuts_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoriquesStatuts_AncienStatutId",
                table: "HistoriquesStatuts",
                column: "AncienStatutId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriquesStatuts_NouveauStatutId",
                table: "HistoriquesStatuts",
                column: "NouveauStatutId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriquesStatuts_ReclamationId",
                table: "HistoriquesStatuts",
                column: "ReclamationId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriquesStatuts_UtilisateurId",
                table: "HistoriquesStatuts",
                column: "UtilisateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoriquesStatuts");
        }
    }
}
