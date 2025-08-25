using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserToIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"AspNetUsers\" SET \"Email\" = 'admin@admin.admin' WHERE \"UserName\" = 'admin';"
            );
            migrationBuilder.Sql(
                "UPDATE \"AspNetUsers\" SET \"NormalizedEmail\" = 'ADMIN@ADMIN.ADMIN' WHERE \"UserName\" = 'admin';"
            );
            migrationBuilder.Sql("UPDATE \"AspNetUsers\" SET \"UserName\" = \"Email\";");
            migrationBuilder.Sql(
                "UPDATE \"AspNetUsers\" SET \"NormalizedUserName\" = \"NormalizedEmail\";"
            );

            if (migrationBuilder.IsNpgsql())
            {
                migrationBuilder.Sql(
                    "UPDATE \"Logs\" l SET \"AuthorEmail\" = \"AspNetUsers\".\"Email\" FROM \"AspNetUsers\" where \"AuthorId\" = \"AspNetUsers\".\"Id\" and \"AspNetUsers\".\"Email\" is not null and \"AuthorId\" is not null;"
                );
            }

            migrationBuilder.DropColumn(name: "Name", table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: true
            );
        }
    }
}
