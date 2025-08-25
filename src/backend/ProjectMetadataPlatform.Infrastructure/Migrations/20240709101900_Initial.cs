using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    PluginName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    ClientName = table.Column<string>(type: "text", nullable: false),
                    BusinessUnit = table.Column<string>(type: "text", nullable: false),
                    TeamNumber = table.Column<int>(type: "integer", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProjectPluginsRelation",
                columns: table => new
                {
                    PluginId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ProjectPluginsRelation",
                        x => new
                        {
                            x.PluginId,
                            x.ProjectId,
                            x.Url,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_ProjectPluginsRelation_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ProjectPluginsRelation_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "Plugins",
                columns: new[] { "Id", "IsArchived", "PluginName" },
                values: new object[,]
                {
                    { 100, false, "Gitlab" },
                    { 200, false, "SonarQube" },
                    { 300, false, "Jira" },
                }
            );

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[]
                {
                    "Id",
                    "BusinessUnit",
                    "ClientName",
                    "Department",
                    "ProjectName",
                    "TeamNumber",
                },
                values: new object[,]
                {
                    { 100, "Unit 1", "Deutsche Bahn", "Department 1", "DB App", 1 },
                    { 200, "Unit 2", "ARD", "Department 2", "Tagesschau App", 2 },
                    { 300, "Unit 3", "AOK", "Department 3", "AOK Bonus App", 3 },
                }
            );

            migrationBuilder.InsertData(
                table: "ProjectPluginsRelation",
                columns: new[] { "PluginId", "ProjectId", "Url", "DisplayName" },
                values: new object[,]
                {
                    { 100, 100, "https://http.cat/status/100", "Gitlab" },
                    { 100, 200, "https://http.cat/status/204", "Gitlab" },
                    { 100, 300, "https://http.cat/status/406", "Gitlab" },
                    { 200, 100, "https://http.cat/status/102", "SonarQube" },
                    { 200, 200, "https://http.cat/status/401", "SonarQube" },
                    { 200, 300, "https://http.cat/status/411", "SonarQube" },
                    { 300, 100, "https://http.cat/status/200", "Jira" },
                    { 300, 200, "https://http.cat/status/404", "Jira" },
                    { 300, 300, "https://http.cat/status/414", "Jira" },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPluginsRelation_ProjectId",
                table: "ProjectPluginsRelation",
                column: "ProjectId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProjectPluginsRelation");

            migrationBuilder.DropTable(name: "Plugins");

            migrationBuilder.DropTable(name: "Projects");
        }
    }
}
