using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProjectAttributesToTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Company", "CompanyState", "OfferId" },
                values: new object[] { "AppsFactory", 1, "Offer1" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "Company", "IsmsLevel", "OfferId" },
                values: new object[] { "AppsCompany", 1, "Offer2" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "Company", "CompanyState", "IsmsLevel", "OfferId" },
                values: new object[] { "AppsFactory", 1, 2, "Offer3" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Company", "CompanyState", "OfferId" },
                values: new object[] { "", 0, "" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "Company", "IsmsLevel", "OfferId" },
                values: new object[] { "", 0, "" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "Company", "CompanyState", "IsmsLevel", "OfferId" },
                values: new object[] { "", 0, 0, "" }
            );
        }
    }
}
