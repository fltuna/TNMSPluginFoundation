using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TnmsAdministrationPlatform.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tnms_admin_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    group_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_server",
                columns: table => new
                {
                    server_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_server", x => x.server_name);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_user",
                columns: table => new
                {
                    steam_id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    immunity = table.Column<byte>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_user", x => x.steam_id);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_group_permission_global",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    group_id = table.Column<int>(type: "INTEGER", nullable: false),
                    permission_node = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_group_permission_global", x => x.id);
                    table.ForeignKey(
                        name: "FK_tnms_admin_group_permission_global_tnms_admin_group_group_id",
                        column: x => x.group_id,
                        principalTable: "tnms_admin_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_group_permission_server",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    group_id = table.Column<int>(type: "INTEGER", nullable: false),
                    permission_node = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    server_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_group_permission_server", x => x.id);
                    table.ForeignKey(
                        name: "FK_tnms_admin_group_permission_server_tnms_admin_group_group_id",
                        column: x => x.group_id,
                        principalTable: "tnms_admin_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tnms_admin_group_permission_server_tnms_admin_server_server_name",
                        column: x => x.server_name,
                        principalTable: "tnms_admin_server",
                        principalColumn: "server_name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_group_relation",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    group_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_steam_id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_group_relation", x => x.id);
                    table.ForeignKey(
                        name: "FK_tnms_admin_group_relation_tnms_admin_group_group_id",
                        column: x => x.group_id,
                        principalTable: "tnms_admin_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tnms_admin_group_relation_tnms_admin_user_user_steam_id",
                        column: x => x.user_steam_id,
                        principalTable: "tnms_admin_user",
                        principalColumn: "steam_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_user_permission_global",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_steam_id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    permission_node = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_user_permission_global", x => x.id);
                    table.ForeignKey(
                        name: "FK_tnms_admin_user_permission_global_tnms_admin_user_user_steam_id",
                        column: x => x.user_steam_id,
                        principalTable: "tnms_admin_user",
                        principalColumn: "steam_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tnms_admin_user_permission_server",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_steam_id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    permission_node = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    server_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tnms_admin_user_permission_server", x => x.id);
                    table.ForeignKey(
                        name: "FK_tnms_admin_user_permission_server_tnms_admin_server_server_name",
                        column: x => x.server_name,
                        principalTable: "tnms_admin_server",
                        principalColumn: "server_name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tnms_admin_user_permission_server_tnms_admin_user_user_steam_id",
                        column: x => x.user_steam_id,
                        principalTable: "tnms_admin_user",
                        principalColumn: "steam_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tnms_admin_group_group_name",
                table: "tnms_admin_group",
                column: "group_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_group_perm_global_group_id",
                table: "tnms_admin_group_permission_global",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "uk_group_perm_global",
                table: "tnms_admin_group_permission_global",
                columns: new[] { "group_id", "permission_node" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_group_perm_server_group_id",
                table: "tnms_admin_group_permission_server",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "idx_group_perm_server_name",
                table: "tnms_admin_group_permission_server",
                column: "server_name");

            migrationBuilder.CreateIndex(
                name: "uk_group_perm_server",
                table: "tnms_admin_group_permission_server",
                columns: new[] { "group_id", "permission_node", "server_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_group_relation_group_id",
                table: "tnms_admin_group_relation",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "idx_group_relation_steam_id",
                table: "tnms_admin_group_relation",
                column: "user_steam_id");

            migrationBuilder.CreateIndex(
                name: "uk_group_user",
                table: "tnms_admin_group_relation",
                columns: new[] { "group_id", "user_steam_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_perm_global_steam_id",
                table: "tnms_admin_user_permission_global",
                column: "user_steam_id");

            migrationBuilder.CreateIndex(
                name: "uk_user_perm_global",
                table: "tnms_admin_user_permission_global",
                columns: new[] { "user_steam_id", "permission_node" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_perm_server_name",
                table: "tnms_admin_user_permission_server",
                column: "server_name");

            migrationBuilder.CreateIndex(
                name: "idx_user_perm_server_steam_id",
                table: "tnms_admin_user_permission_server",
                column: "user_steam_id");

            migrationBuilder.CreateIndex(
                name: "uk_user_perm_server",
                table: "tnms_admin_user_permission_server",
                columns: new[] { "user_steam_id", "permission_node", "server_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tnms_admin_group_permission_global");

            migrationBuilder.DropTable(
                name: "tnms_admin_group_permission_server");

            migrationBuilder.DropTable(
                name: "tnms_admin_group_relation");

            migrationBuilder.DropTable(
                name: "tnms_admin_user_permission_global");

            migrationBuilder.DropTable(
                name: "tnms_admin_user_permission_server");

            migrationBuilder.DropTable(
                name: "tnms_admin_group");

            migrationBuilder.DropTable(
                name: "tnms_admin_server");

            migrationBuilder.DropTable(
                name: "tnms_admin_user");
        }
    }
}
