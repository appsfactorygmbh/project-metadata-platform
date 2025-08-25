using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseUrlToGlobalPlugin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "Plugins",
                type: "text",
                nullable: true
            );

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: 100,
                column: "BaseUrl",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: 200,
                column: "BaseUrl",
                value: null
            );

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: 300,
                column: "BaseUrl",
                value: null
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BaseUrl", table: "Plugins");
        }
    }
}
