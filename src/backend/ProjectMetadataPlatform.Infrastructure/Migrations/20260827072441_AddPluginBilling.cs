using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPluginBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectPluginsRelation",
                table: "ProjectPluginsRelation"
            );

            migrationBuilder.DropIndex(
                name: "IX_ProjectPluginsRelation_ProjectId",
                table: "ProjectPluginsRelation"
            );

            migrationBuilder.DropColumn(name: "OfferId", table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProjectPluginsRelation",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            if (
                migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL"
                || migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.Sqlite"
            )
            {
                migrationBuilder.Sql(
                    @"UPDATE ""ProjectPluginsRelation""
      SET ""Id"" = sub.new_id
      FROM (
          SELECT ""PluginId"", ""ProjectId"", ""Url"",
                 ROW_NUMBER() OVER(PARTITION BY ""ProjectId"" ORDER BY ""PluginId"") as new_id
          FROM ""ProjectPluginsRelation""
      ) sub
      WHERE ""ProjectPluginsRelation"".""PluginId"" = sub.""PluginId""
        AND ""ProjectPluginsRelation"".""ProjectId"" = sub.""ProjectId""
        AND ""ProjectPluginsRelation"".""Url"" = sub.""Url"";"
                );
            }

            migrationBuilder.AddColumn<int>(
                name: "BillingId",
                table: "ProjectPluginsRelation",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "GlobalBillingId",
                table: "Logs",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "GlobalBillingKind",
                table: "Logs",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectPluginsRelation",
                table: "ProjectPluginsRelation",
                columns: new[] { "ProjectId", "Id" }
            );

            migrationBuilder.CreateTable(
                name: "GlobalBilling",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    BillingKind = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    BudgetLimit = table.Column<decimal>(type: "numeric", nullable: true),
                    HostingFee = table.Column<decimal>(type: "numeric", nullable: true),
                    TargetMargin = table.Column<int>(type: "integer", nullable: true),
                    TimeFrame = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalBilling", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "PluginBillingRelation",
                columns: table => new
                {
                    PluginId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    BillingId = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    BudgetLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    HostingFee = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetMargin = table.Column<int>(type: "integer", nullable: false),
                    TimeFrame = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    Notes = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PluginBillingRelation",
                        x => new { x.ProjectId, x.PluginId }
                    );
                    table.ForeignKey(
                        name: "FK_PluginBillingRelation_GlobalBilling_BillingId",
                        column: x => x.BillingId,
                        principalTable: "GlobalBilling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_PluginBillingRelation_ProjectPluginsRelation_ProjectId_Plug~",
                        columns: x => new { x.ProjectId, x.PluginId },
                        principalTable: "ProjectPluginsRelation",
                        principalColumns: new[] { "ProjectId", "Id" },
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPluginsRelation_PluginId_ProjectId_Url",
                table: "ProjectPluginsRelation",
                columns: new[] { "PluginId", "ProjectId", "Url" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_GlobalBillingId",
                table: "Logs",
                column: "GlobalBillingId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GlobalBilling_BillingKind",
                table: "GlobalBilling",
                column: "BillingKind",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_PluginBillingRelation_BillingId",
                table: "PluginBillingRelation",
                column: "BillingId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_GlobalBilling_GlobalBillingId",
                table: "Logs",
                column: "GlobalBillingId",
                principalTable: "GlobalBilling",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_GlobalBilling_GlobalBillingId",
                table: "Logs"
            );

            migrationBuilder.DropTable(name: "PluginBillingRelation");

            migrationBuilder.DropTable(name: "GlobalBilling");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectPluginsRelation",
                table: "ProjectPluginsRelation"
            );

            migrationBuilder.DropIndex(
                name: "IX_ProjectPluginsRelation_PluginId_ProjectId_Url",
                table: "ProjectPluginsRelation"
            );

            migrationBuilder.DropIndex(name: "IX_Logs_GlobalBillingId", table: "Logs");

            migrationBuilder.DropColumn(name: "Id", table: "ProjectPluginsRelation");

            migrationBuilder.DropColumn(name: "BillingId", table: "ProjectPluginsRelation");

            migrationBuilder.DropColumn(name: "GlobalBillingId", table: "Logs");

            migrationBuilder.DropColumn(name: "GlobalBillingKind", table: "Logs");

            migrationBuilder.AddColumn<string>(
                name: "OfferId",
                table: "Projects",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectPluginsRelation",
                table: "ProjectPluginsRelation",
                columns: new[] { "PluginId", "ProjectId", "Url" }
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 100,
                column: "OfferId",
                value: "Offer1"
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 200,
                column: "OfferId",
                value: "Offer2"
            );

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 300,
                column: "OfferId",
                value: "Offer3"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPluginsRelation_ProjectId",
                table: "ProjectPluginsRelation",
                column: "ProjectId"
            );
        }
    }
}
