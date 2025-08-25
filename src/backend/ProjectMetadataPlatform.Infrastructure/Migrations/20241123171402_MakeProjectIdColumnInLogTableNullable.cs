using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeProjectIdColumnInLogTableNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Logs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            if (migrationBuilder.IsNpgsql())
            {
                migrationBuilder.Sql(
                    "UPDATE \"Logs\" l SET \"ProjectName\" = \"Projects\".\"ProjectName\" FROM \"Projects\" where \"ProjectId\" = \"Projects\".\"Id\" and l.\"ProjectName\" is null and \"ProjectId\" is not null;"
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Logs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true
            );
        }
    }
}
