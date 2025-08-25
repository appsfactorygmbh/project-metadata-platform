using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamLinkToProjectAndRemoveMetadataFromProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BusinessUnit", table: "Projects");

            migrationBuilder.DropColumn(name: "Department", table: "Projects");

            migrationBuilder.DropColumn(name: "TeamNumber", table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Projects",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    TeamName = table.Column<string>(type: "text", nullable: false),
                    BusinessUnit = table.Column<string>(type: "text", nullable: false),
                    PTL = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                column: "TeamId",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                column: "TeamId",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                column: "TeamId",
                value: null
            );

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TeamId",
                table: "Projects",
                column: "TeamId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Team_TeamName",
                table: "Team",
                column: "TeamName",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Team_TeamId",
                table: "Projects",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Projects_Team_TeamId", table: "Projects");

            migrationBuilder.DropTable(name: "Team");

            migrationBuilder.DropIndex(name: "IX_Projects_TeamId", table: "Projects");

            migrationBuilder.DropColumn(name: "TeamId", table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "BusinessUnit",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "TeamNumber",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "BusinessUnit", "Department", "TeamNumber" },
                values: new object[] { "Unit 1", "Department 1", 1 }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "BusinessUnit", "Department", "TeamNumber" },
                values: new object[] { "Unit 2", "Department 2", 2 }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "BusinessUnit", "Department", "TeamNumber" },
                values: new object[] { "Unit 3", "Department 3", 3 }
            );
        }
    }
}
