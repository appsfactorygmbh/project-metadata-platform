using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            if (migrationBuilder.IsNpgsql())
            {
                migrationBuilder.Sql(
                    "UPDATE \"Projects\" SET \"Slug\" = LOWER(REPLACE(TRIM(REGEXP_REPLACE(\"ProjectName\", '[^a-zA-Z0-9_ ]', '')), ' ', '_'));"
                );
            }
            else
            {
                migrationBuilder.Sql(
                    "UPDATE \"Projects\" SET \"Slug\" = LOWER(REPLACE(TRIM(\"ProjectName\"), ' ', '_'));"
                );
            }

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Slug",
                table: "Projects",
                column: "Slug",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Projects_Slug", table: "Projects");

            migrationBuilder.DropColumn(name: "Slug", table: "Projects");
        }
    }
}
