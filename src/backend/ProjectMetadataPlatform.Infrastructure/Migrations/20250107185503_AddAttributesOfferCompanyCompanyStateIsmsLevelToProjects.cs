using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributesOfferCompanyCompanyStateIsmsLevelToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "CompanyState",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "IsmsLevel",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "OfferId",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Company", "CompanyState", "IsmsLevel", "OfferId" },
                values: new object[] { "", 0, 0, "" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "Company", "CompanyState", "IsmsLevel", "OfferId" },
                values: new object[] { "", 0, 0, "" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "Company", "CompanyState", "IsmsLevel", "OfferId" },
                values: new object[] { "", 0, 0, "" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Company", table: "Projects");

            migrationBuilder.DropColumn(name: "CompanyState", table: "Projects");

            migrationBuilder.DropColumn(name: "IsmsLevel", table: "Projects");

            migrationBuilder.DropColumn(name: "OfferId", table: "Projects");
        }
    }
}
