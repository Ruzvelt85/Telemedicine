using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemedicine.Services.VideoConfIntegrService.MigrationEfCore.Migrations
{
    public partial class Version_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conference",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    appointment_start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    appointment_duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    full_extension = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    room_id = table.Column<int>(type: "integer", nullable: false),
                    room_url = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    room_pin = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    xml_response = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conference", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_conference_appointment_id",
                table: "conference",
                column: "appointment_id",
                unique: true,
                filter: "is_deleted = false");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conference");
        }
    }
}
