using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LogRepositoryChangesForTeamReferencing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Projects_Team_TeamId", table: "Projects");

            migrationBuilder.DropPrimaryKey(name: "PK_Team", table: "Team");

            migrationBuilder.RenameTable(name: "Team", newName: "Teams");

            migrationBuilder.RenameIndex(
                name: "IX_Team_TeamName",
                table: "Teams",
                newName: "IX_Teams_TeamName"
            );

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "TeamName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddPrimaryKey(name: "PK_Teams", table: "Teams", column: "Id");

            migrationBuilder.CreateIndex(name: "IX_Logs_TeamId", table: "Logs", column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Teams_TeamId",
                table: "Logs",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Teams_TeamId",
                table: "Projects",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Logs_Teams_TeamId", table: "Logs");

            migrationBuilder.DropForeignKey(name: "FK_Projects_Teams_TeamId", table: "Projects");

            migrationBuilder.DropIndex(name: "IX_Logs_TeamId", table: "Logs");

            migrationBuilder.DropPrimaryKey(name: "PK_Teams", table: "Teams");

            migrationBuilder.DropColumn(name: "TeamId", table: "Logs");

            migrationBuilder.DropColumn(name: "TeamName", table: "Logs");

            migrationBuilder.RenameTable(name: "Teams", newName: "Team");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_TeamName",
                table: "Team",
                newName: "IX_Team_TeamName"
            );

            migrationBuilder.AddPrimaryKey(name: "PK_Team", table: "Team", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Team_TeamId",
                table: "Projects",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
