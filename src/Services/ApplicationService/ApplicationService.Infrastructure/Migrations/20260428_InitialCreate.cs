using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationService.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema("application");

        migrationBuilder.CreateTable(
            name: "applications",
            schema: "application",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                job_id = table.Column<Guid>(type: "uuid", nullable: false),
                candidate_id = table.Column<Guid>(type: "uuid", nullable: false),
                recruiter_id = table.Column<Guid>(type: "uuid", nullable: false),
                resume_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                cover_letter = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                applied_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_applications", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ux_applications_job_candidate",
            schema: "application",
            table: "applications",
            columns: new[] { "job_id", "candidate_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_applications_candidate_id",
            schema: "application",
            table: "applications",
            column: "candidate_id");

        migrationBuilder.CreateIndex(
            name: "ix_applications_job_id",
            schema: "application",
            table: "applications",
            column: "job_id");

        migrationBuilder.CreateIndex(
            name: "ix_applications_recruiter_id",
            schema: "application",
            table: "applications",
            column: "recruiter_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "applications",
            schema: "application");
    }
}
