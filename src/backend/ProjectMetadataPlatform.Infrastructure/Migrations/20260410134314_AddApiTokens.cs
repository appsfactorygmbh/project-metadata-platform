using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApiTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorEmail",
                table: "Logs",
                newName: "AuthorName"
            );

            migrationBuilder.AddColumn<int>(
                name: "AffectedTokenId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "AffectedTokenName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AuthorTokenId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "ApiTokens",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Scopes = table.Column<int[]>(type: "integer[]", nullable: true),
                    ExpirationDate = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_AffectedTokenId",
                table: "Logs",
                column: "AffectedTokenId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_AuthorTokenId",
                table: "Logs",
                column: "AuthorTokenId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_Name",
                table: "ApiTokens",
                column: "Name",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_ApiTokens_AffectedTokenId",
                table: "Logs",
                column: "AffectedTokenId",
                principalTable: "ApiTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_ApiTokens_AuthorTokenId",
                table: "Logs",
                column: "AuthorTokenId",
                principalTable: "ApiTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_ApiTokens_AffectedTokenId",
                table: "Logs"
            );

            migrationBuilder.DropForeignKey(name: "FK_Logs_ApiTokens_AuthorTokenId", table: "Logs");

            migrationBuilder.DropTable(name: "ApiTokens");

            migrationBuilder.DropIndex(name: "IX_Logs_AffectedTokenId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_AuthorTokenId", table: "Logs");

            migrationBuilder.DropColumn(name: "AffectedTokenId", table: "Logs");

            migrationBuilder.DropColumn(name: "AffectedTokenName", table: "Logs");

            migrationBuilder.DropColumn(name: "AuthorTokenId", table: "Logs");

            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Logs",
                newName: "AuthorEmail"
            );
        }
    }
}
