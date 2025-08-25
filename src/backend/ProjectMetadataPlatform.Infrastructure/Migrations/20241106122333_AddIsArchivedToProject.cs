using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                column: "IsArchived",
                value: false
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                column: "IsArchived",
                value: false
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                column: "IsArchived",
                value: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsArchived", table: "Projects");
        }
    }
}
