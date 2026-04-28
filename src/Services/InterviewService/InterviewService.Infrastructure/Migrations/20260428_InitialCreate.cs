using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InterviewService.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema("interview");

        migrationBuilder.CreateTable(
            name: "interviews",
            schema: "interview",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                application_id = table.Column<Guid>(type: "uuid", nullable: false),
                candidate_id = table.Column<Guid>(type: "uuid", nullable: false),
                recruiter_id = table.Column<Guid>(type: "uuid", nullable: false),
                scheduled_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                mode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                meeting_link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_interviews", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ux_interviews_application_id",
            schema: "interview",
            table: "interviews",
            column: "application_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_interviews_candidate_id",
            schema: "interview",
            table: "interviews",
            column: "candidate_id");

        migrationBuilder.CreateIndex(
            name: "ix_interviews_recruiter_id",
            schema: "interview",
            table: "interviews",
            column: "recruiter_id");

        migrationBuilder.CreateIndex(
            name: "ix_interviews_scheduled_at",
            schema: "interview",
            table: "interviews",
            column: "scheduled_at");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "interviews",
            schema: "interview");
    }
}
