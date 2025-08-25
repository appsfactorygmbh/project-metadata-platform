using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandLogsToPluginsAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Logs_AspNetUsers_UserId", table: "Logs");

            migrationBuilder.DropForeignKey(name: "FK_Logs_Projects_ProjectId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_UserId", table: "Logs");

            migrationBuilder.RenameColumn(name: "UserId", table: "Logs", newName: "AuthorId");

            migrationBuilder.RenameColumn(name: "Email", table: "Logs", newName: "AuthorEmail");

            migrationBuilder.AddColumn<string>(
                name: "AffectedUserEmail",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "AffectedUserId",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "GlobalPluginName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "GlobalPluginId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_AffectedUserId",
                table: "Logs",
                column: "AffectedUserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_AuthorId",
                table: "Logs",
                column: "AuthorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_GlobalPluginId",
                table: "Logs",
                column: "GlobalPluginId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_AffectedUserId",
                table: "Logs",
                column: "AffectedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_AuthorId",
                table: "Logs",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Plugins_GlobalPluginId",
                table: "Logs",
                column: "GlobalPluginId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Projects_ProjectId",
                table: "Logs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_AspNetUsers_AffectedUserId",
                table: "Logs"
            );

            migrationBuilder.DropForeignKey(name: "FK_Logs_AspNetUsers_AuthorId", table: "Logs");

            migrationBuilder.DropForeignKey(name: "FK_Logs_Plugins_GlobalPluginId", table: "Logs");

            migrationBuilder.DropForeignKey(name: "FK_Logs_Projects_ProjectId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_AffectedUserId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_AuthorId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_GlobalPluginId", table: "Logs");

            migrationBuilder.DropColumn(name: "AffectedUserEmail", table: "Logs");

            migrationBuilder.DropColumn(name: "AffectedUserId", table: "Logs");

            migrationBuilder.DropColumn(name: "GlobalPluginName", table: "Logs");

            migrationBuilder.DropColumn(name: "ProjectName", table: "Logs");

            migrationBuilder.DropColumn(name: "GlobalPluginId", table: "Logs");

            migrationBuilder.RenameColumn(name: "AuthorId", table: "Logs", newName: "UserId");

            migrationBuilder.RenameColumn(name: "AuthorEmail", table: "Logs", newName: "Email");

            migrationBuilder.CreateIndex(name: "IX_Logs_UserId", table: "Logs", column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_AspNetUsers_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Projects_ProjectId",
                table: "Logs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
