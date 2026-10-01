using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "CompanyStateLookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyStateLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "CurrencyLookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "SecurityLevelLookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityLevelLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "TimeFrameLookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeFrameLookup", x => x.Id);
                }
            );

            migrationBuilder.InsertData(
                table: "CompanyStateLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "external" },
                    { 1, "internal" },
                }
            );

            migrationBuilder.InsertData(
                table: "CurrencyLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 849, "usd" },
                    { 978, "eur" },
                }
            );

            migrationBuilder.InsertData(
                table: "SecurityLevelLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "normal" },
                    { 1, "high" },
                    { 2, "very high" },
                }
            );

            migrationBuilder.InsertData(
                table: "TimeFrameLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "monthly" },
                    { 1, "quarterly" },
                    { 2, "yearly" },
                    { 3, "date" },
                    { 4, "never" },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logs_TimeStamp_Id",
                table: "Logs",
                columns: new[] { "TimeStamp", "Id" },
                descending: new bool[0]
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email_EmployeeId",
                table: "AspNetUsers",
                columns: new[] { "Email", "EmployeeId" }
            );

            migrationBuilder.Sql(
                @"
CREATE MATERIALIZED VIEW mv_project_search AS
SELECT
    p.""Id"",
    LOWER(p.""Slug"") AS ""Slug"",
    LOWER(p.""ProjectName"") AS ""ProjectName"",
    LOWER(p.""ClientName"") AS ""ClientName"",
    LOWER(p.""Notes"") AS ""Notes"",
    p.""IsArchived"",
    p.""IsEoC"",
    LOWER(c.""CompanyName"") AS ""CompanyName"",
    LOWER(t.""TeamName"") AS ""TeamName"",
    LOWER(b.""BusinessUnitName"") AS ""BusinessUnitName"",
    LOWER(csl.""Name"") AS ""CompanyStateText"",
    LOWER(sec.""Name"") AS ""SecurityLevelText"",
    LOWER(COALESCE(
        (
            SELECT string_agg(
                CONCAT_WS(' ', pl.""PluginName"", pp.""Url"", pp.""DisplayName""), ' '
            )
            FROM ""ProjectPluginsRelation"" pp
            LEFT JOIN ""Plugins"" pl ON pp.""PluginId"" = pl.""Id""
            WHERE pp.""ProjectId"" = p.""Id""
        ), ''
    )) AS ""PluginsSearchText"",
    LOWER(COALESCE(
        (
            SELECT string_agg(
                CONCAT_WS(' ',
                    array_to_string(pb.""ContractIds"", ' '),
                    CAST(pb.""BudgetLimit"" AS TEXT),
                    CAST(pb.""HostingFee"" AS TEXT),
                    CAST(pb.""TargetMargin"" AS TEXT),
                    CAST(pb.""Date"" AS TEXT),
                    pb.""Notes"",
                    curr.""Name"",
                    tf.""Name""
                ), ' '
            )
            FROM ""PluginBillingRelation"" pb
            LEFT JOIN ""CurrencyLookup"" curr ON pb.""Currency"" = curr.""Id""
            LEFT JOIN ""TimeFrameLookup"" tf ON pb.""TimeFrame"" = tf.""Id""
            WHERE pb.""ProjectId"" = p.""Id""
        ), ''
    )) AS ""BillingSearchText""

FROM ""Projects"" p
LEFT JOIN ""Companies"" c ON p.""CompanyId"" = c.""Id""
LEFT JOIN ""Teams"" t ON p.""TeamId"" = t.""Id""
LEFT JOIN ""BusinessUnits"" b ON t.""BusinessUnitId"" = b.""Id""

LEFT JOIN ""CompanyStateLookup"" csl ON p.""CompanyState"" = csl.""Id""
LEFT JOIN ""SecurityLevelLookup"" sec ON p.""IsmsLevel"" = sec.""Id"";
"
            );

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX idx_mv_project_id ON mv_project_search(""Id"");"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_client_sort ON mv_project_search(""ClientName"", ""Slug"");"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_name_sort ON mv_project_search(""ProjectName"", ""Slug"");"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_company_sort ON mv_project_search(""CompanyName"", ""Slug"");"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_team_sort ON mv_project_search(""TeamName"", ""Slug"");"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_bu_sort ON mv_project_search(""BusinessUnitName"", ""Slug"");"
            );

            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_plugins_trgm ON mv_project_search USING gin (""PluginsSearchText"" gin_trgm_ops);"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_billing_trgm ON mv_project_search USING gin (""BillingSearchText"" gin_trgm_ops);"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_projectname_trgm ON mv_project_search USING gin (""ProjectName"" gin_trgm_ops);"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_clientname_trgm ON mv_project_search USING gin (""ClientName"" gin_trgm_ops);"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_companyname_trgm ON mv_project_search USING gin (""CompanyName"" gin_trgm_ops);"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_notes_trgm ON mv_project_search USING gin (""Notes"" gin_trgm_ops);"
            );

            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_companystate_trgm ON mv_project_search USING gin (""CompanyStateText"" gin_trgm_ops);"
            );
            migrationBuilder.Sql(
                @"CREATE INDEX idx_mv_project_securitylevel_trgm ON mv_project_search USING gin (""SecurityLevelText"" gin_trgm_ops);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS mv_project_search;");
            migrationBuilder.DropTable(name: "CompanyStateLookup");

            migrationBuilder.DropTable(name: "CurrencyLookup");

            migrationBuilder.DropTable(name: "SecurityLevelLookup");

            migrationBuilder.DropTable(name: "TimeFrameLookup");

            migrationBuilder.DropIndex(name: "IX_Logs_TimeStamp_Id", table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email_EmployeeId",
                table: "AspNetUsers"
            );

            migrationBuilder
                .AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
