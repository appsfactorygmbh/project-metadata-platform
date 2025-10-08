using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations.CasbinDb
{
    /// <inheritdoc />
    public partial class CasbinAbac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "casbin_rule",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ptype = table.Column<string>(type: "text", nullable: true),
                    v0 = table.Column<string>(type: "text", nullable: true),
                    v1 = table.Column<string>(type: "text", nullable: true),
                    v2 = table.Column<string>(type: "text", nullable: true),
                    v3 = table.Column<string>(type: "text", nullable: true),
                    v4 = table.Column<string>(type: "text", nullable: true),
                    v5 = table.Column<string>(type: "text", nullable: true),
                    Value13 = table.Column<string>(type: "text", nullable: true),
                    Value14 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_casbin_rule", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_ptype",
                table: "casbin_rule",
                column: "ptype");

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_v0",
                table: "casbin_rule",
                column: "v0");

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_v1",
                table: "casbin_rule",
                column: "v1");

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_v2",
                table: "casbin_rule",
                column: "v2");

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_v3",
                table: "casbin_rule",
                column: "v3");

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_v4",
                table: "casbin_rule",
                column: "v4");

            migrationBuilder.CreateIndex(
                name: "IX_casbin_rule_v5",
                table: "casbin_rule",
                column: "v5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "casbin_rule");
        }
    }
}
