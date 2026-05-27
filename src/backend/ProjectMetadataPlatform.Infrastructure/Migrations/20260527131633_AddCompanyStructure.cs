using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessUnitId",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "BusinessUnitId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "BusinessUnitName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "OfficeLocationId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "OfficeLocationName",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "IsScimProvisioned",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "OfficeLocationId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "BusinessUnits",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    BusinessUnitName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnits", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    DepartmentName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "OfficeLocations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    OfficeLocationName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeLocations", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ApplicationUserBusinessUnit",
                columns: table => new
                {
                    BusinessUnitsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ApplicationUserBusinessUnit",
                        x => new { x.BusinessUnitsId, x.UsersId }
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserBusinessUnit_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserBusinessUnit_BusinessUnits_BusinessUnitsId",
                        column: x => x.BusinessUnitsId,
                        principalTable: "BusinessUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ApplicationUserDepartment",
                columns: table => new
                {
                    DepartmentsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ApplicationUserDepartment",
                        x => new { x.DepartmentsId, x.UsersId }
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserDepartment_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserDepartment_Departments_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "CompanyName" },
                values: new object[,]
                {
                    { 1, "AppsFactory" },
                    { 2, "AppsCompany" },
                }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                column: "CompanyId",
                value: 1
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                column: "CompanyId",
                value: 2
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                column: "CompanyId",
                value: 1
            );

            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    @"
        SELECT setval(pg_get_serial_sequence('""Companies""', 'Id'), (SELECT MAX(""Id"") FROM ""Companies""));
    "
                );
            }

            migrationBuilder.Sql(
                @"
    INSERT INTO ""Companies"" (""CompanyName"")
    SELECT DISTINCT ""Company"" FROM ""Projects""
    WHERE ""Company"" IS NOT NULL
      AND ""Company"" <> ''
      AND ""Company"" NOT IN (SELECT ""CompanyName"" FROM ""Companies"");

    UPDATE ""Projects""
    SET ""CompanyId"" = (
        SELECT ""Id""
        FROM ""Companies""
        WHERE ""Companies"".""CompanyName"" = ""Projects"".""Company""
    );
"
            );

            migrationBuilder.Sql(
                @"
        INSERT INTO ""BusinessUnits"" (""BusinessUnitName"")
        SELECT DISTINCT ""BusinessUnit"" FROM ""Teams"" WHERE ""BusinessUnit"" IS NOT NULL AND ""BusinessUnit"" <> '';

        UPDATE ""Teams""
        SET ""BusinessUnitId"" = (SELECT ""Id"" FROM ""BusinessUnits"" WHERE ""BusinessUnits"".""BusinessUnitName"" = ""Teams"".""BusinessUnit"");
    "
            );
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    @"
        -- Extract distinct departments from the text[] array
        INSERT INTO ""Departments"" (""DepartmentName"")
        SELECT DISTINCT unnest(""Departments"") FROM ""AspNetUsers"" WHERE ""Departments"" IS NOT NULL;

        -- Map users to departments in the junction table
        INSERT INTO ""ApplicationUserDepartment"" (""UsersId"", ""DepartmentsId"")
        SELECT u.""Id"", d.""Id""
        FROM ""AspNetUsers"" u
        CROSS JOIN unnest(u.""Departments"") AS dept_name
        JOIN ""Departments"" d ON d.""DepartmentName"" = dept_name;
    "
                );
            }

            migrationBuilder.DropColumn(name: "BusinessUnit", table: "Teams");

            migrationBuilder.DropColumn(name: "Company", table: "Projects");

            migrationBuilder.DropColumn(name: "BusinessUnits", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "Company", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "Departments", table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_BusinessUnitId",
                table: "Teams",
                column: "BusinessUnitId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects",
                column: "CompanyId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_BusinessUnitId",
                table: "Logs",
                column: "BusinessUnitId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CompanyId",
                table: "Logs",
                column: "CompanyId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_DepartmentId",
                table: "Logs",
                column: "DepartmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OfficeLocationId",
                table: "Logs",
                column: "OfficeLocationId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OfficeLocationId",
                table: "AspNetUsers",
                column: "OfficeLocationId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserBusinessUnit_UsersId",
                table: "ApplicationUserBusinessUnit",
                column: "UsersId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserDepartment_UsersId",
                table: "ApplicationUserDepartment",
                column: "UsersId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnits_BusinessUnitName",
                table: "BusinessUnits",
                column: "BusinessUnitName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyName",
                table: "Companies",
                column: "CompanyName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentName",
                table: "Departments",
                column: "DepartmentName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_OfficeLocations_OfficeLocationName",
                table: "OfficeLocations",
                column: "OfficeLocationName",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_OfficeLocations_OfficeLocationId",
                table: "AspNetUsers",
                column: "OfficeLocationId",
                principalTable: "OfficeLocations",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_BusinessUnits_BusinessUnitId",
                table: "Logs",
                column: "BusinessUnitId",
                principalTable: "BusinessUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Companies_CompanyId",
                table: "Logs",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Departments_DepartmentId",
                table: "Logs",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_OfficeLocations_OfficeLocationId",
                table: "Logs",
                column: "OfficeLocationId",
                principalTable: "OfficeLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_BusinessUnits_BusinessUnitId",
                table: "Teams",
                column: "BusinessUnitId",
                principalTable: "BusinessUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessUnit",
                table: "Teams",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AlterColumn<bool>(
                name: "IsScimProvisioned",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean"
            );

            migrationBuilder.AddColumn<List<string>>(
                name: "BusinessUnits",
                table: "AspNetUsers",
                type: "text[]",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "AspNetUsers",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<List<string>>(
                name: "Departments",
                table: "AspNetUsers",
                type: "text[]",
                nullable: true
            );

            migrationBuilder.Sql(
                @"
        UPDATE ""Projects""
        SET ""Company"" = (
            SELECT ""CompanyName""
            FROM ""Companies""
            WHERE ""Companies"".""Id"" = ""Projects"".""CompanyId""
        );
    "
            );

            migrationBuilder.Sql(
                @"
        UPDATE ""Teams""
        SET ""BusinessUnit"" = (
            SELECT ""BusinessUnitName""
            FROM ""BusinessUnits""
            WHERE ""BusinessUnits"".""Id"" = ""Teams"".""BusinessUnitId""
        );
    "
            );
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    @"
        UPDATE ""AspNetUsers""
        SET ""Departments"" = (
            SELECT ARRAY_AGG(d.""DepartmentName"")
            FROM ""ApplicationUserDepartment"" aud
            JOIN ""Departments"" d ON d.""Id"" = aud.""DepartmentsId""
            WHERE aud.""UsersId"" = ""AspNetUsers"".""Id""
        );
    "
                );

                migrationBuilder.Sql(
                    @"
        UPDATE ""AspNetUsers""
        SET ""BusinessUnits"" = (
            SELECT ARRAY_AGG(bu.""BusinessUnitName"")
            FROM ""ApplicationUserBusinessUnit"" aubu
            JOIN ""BusinessUnits"" bu ON bu.""Id"" = aubu.""BusinessUnitsId""
            WHERE aubu.""UsersId"" = ""AspNetUsers"".""Id""
        );
    "
                );
            }

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_OfficeLocations_OfficeLocationId",
                table: "AspNetUsers"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_BusinessUnits_BusinessUnitId",
                table: "Logs"
            );

            migrationBuilder.DropForeignKey(name: "FK_Logs_Companies_CompanyId", table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Departments_DepartmentId",
                table: "Logs"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_OfficeLocations_OfficeLocationId",
                table: "Logs"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_BusinessUnits_BusinessUnitId",
                table: "Teams"
            );

            migrationBuilder.DropTable(name: "ApplicationUserBusinessUnit");

            migrationBuilder.DropTable(name: "ApplicationUserDepartment");

            migrationBuilder.DropTable(name: "Companies");

            migrationBuilder.DropTable(name: "OfficeLocations");

            migrationBuilder.DropTable(name: "BusinessUnits");

            migrationBuilder.DropTable(name: "Departments");

            migrationBuilder.DropIndex(name: "IX_Teams_BusinessUnitId", table: "Teams");

            migrationBuilder.DropIndex(name: "IX_Projects_CompanyId", table: "Projects");

            migrationBuilder.DropIndex(name: "IX_Logs_BusinessUnitId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_CompanyId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_DepartmentId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_Logs_OfficeLocationId", table: "Logs");

            migrationBuilder.DropIndex(name: "IX_AspNetUsers_CompanyId", table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OfficeLocationId",
                table: "AspNetUsers"
            );

            migrationBuilder.DropColumn(name: "BusinessUnitId", table: "Teams");

            migrationBuilder.DropColumn(name: "CompanyId", table: "Projects");

            migrationBuilder.DropColumn(name: "BusinessUnitId", table: "Logs");

            migrationBuilder.DropColumn(name: "BusinessUnitName", table: "Logs");

            migrationBuilder.DropColumn(name: "CompanyId", table: "Logs");

            migrationBuilder.DropColumn(name: "CompanyName", table: "Logs");

            migrationBuilder.DropColumn(name: "DepartmentId", table: "Logs");

            migrationBuilder.DropColumn(name: "DepartmentName", table: "Logs");

            migrationBuilder.DropColumn(name: "OfficeLocationId", table: "Logs");

            migrationBuilder.DropColumn(name: "OfficeLocationName", table: "Logs");

            migrationBuilder.DropColumn(name: "CompanyId", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "OfficeLocationId", table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                column: "Company",
                value: "AppsFactory"
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                column: "Company",
                value: "AppsCompany"
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                column: "Company",
                value: "AppsFactory"
            );
        }
    }
}
