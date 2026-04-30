using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(
                    @"UPDATE ""AspNetUsers"" SET ""EmployeeId"" = gen_random_uuid()::text WHERE ""EmployeeId"" = '';"
                );
            }
            else if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                    @"UPDATE [AspNetUsers] SET [EmployeeId] = CAST(NEWID() AS NVARCHAR(36)) WHERE [EmployeeId] = '';"
                );
            }
            else if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                migrationBuilder.Sql(
                    @"UPDATE ""AspNetUsers"" SET ""EmployeeId"" = lower(hex(randomblob(16))) WHERE ""EmployeeId"" = '';"
                );
            }
            else if (migrationBuilder.ActiveProvider == "Pomelo.EntityFrameworkCore.MySql")
            {
                migrationBuilder.Sql(
                    @"UPDATE `AspNetUsers` SET `EmployeeId` = UUID() WHERE `EmployeeId` = '';"
                );
            }

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsScimProvisioned",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: true
            );

            migrationBuilder.AddColumn<List<string>>(
                name: "JobTitles",
                table: "AspNetUsers",
                type: "text[]",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "ApplicationUserTeam",
                columns: table => new
                {
                    TeamsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserTeam", x => new { x.TeamsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserTeam_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserTeam_Teams_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ApplicationUserTeam1",
                columns: table => new
                {
                    TeamSupportId = table.Column<int>(type: "integer", nullable: false),
                    TeamSupportUsersId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ApplicationUserTeam1",
                        x => new { x.TeamSupportId, x.TeamSupportUsersId }
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserTeam1_AspNetUsers_TeamSupportUsersId",
                        column: x => x.TeamSupportUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ApplicationUserTeam1_Teams_TeamSupportId",
                        column: x => x.TeamSupportId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserTeam_UsersId",
                table: "ApplicationUserTeam",
                column: "UsersId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserTeam1_TeamSupportUsersId",
                table: "ApplicationUserTeam1",
                column: "TeamSupportUsersId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApplicationUserTeam");

            migrationBuilder.DropTable(name: "ApplicationUserTeam1");

            migrationBuilder.DropIndex(name: "IX_AspNetUsers_EmployeeId", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "BusinessUnits", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "Company", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "Departments", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "EmployeeId", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "IsActive", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "IsScimProvisioned", table: "AspNetUsers");

            migrationBuilder.DropColumn(name: "JobTitles", table: "AspNetUsers");
        }
    }
}
