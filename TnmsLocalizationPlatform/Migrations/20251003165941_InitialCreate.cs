using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnmsLocalizationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_language",
                columns: table => new
                {
                    steam_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    language_code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_language", x => x.steam_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_language_language_code",
                table: "user_language",
                column: "language_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_language");
        }
    }
}
