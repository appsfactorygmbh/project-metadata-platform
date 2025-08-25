using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetDeleteBehaviorOnLogsTableForTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Logs_Teams_TeamId", table: "Logs");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Teams_TeamId",
                table: "Logs",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Logs_Teams_TeamId", table: "Logs");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Teams_TeamId",
                table: "Logs",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id"
            );
        }
    }
}
