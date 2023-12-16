using System;
using System.Collections.Generic;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemedicine.Services.HealthMeasurementDomainService.MigrationEfCore.Migrations
{
    public partial class Version_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blood_pressure_measurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    systolic = table.Column<int>(type: "integer", nullable: false),
                    diastolic = table.Column<int>(type: "integer", nullable: false),
                    pulse_rate = table.Column<int>(type: "integer", nullable: false),
                    client_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blood_pressure_measurement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mood_measurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    measure = table.Column<int>(type: "integer", nullable: false),
                    client_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mood_measurement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pulse_rate_measurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pulse_rate = table.Column<int>(type: "integer", nullable: false),
                    client_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pulse_rate_measurement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saturation_measurement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sp_o2 = table.Column<int>(type: "integer", nullable: false),
                    pulse_rate = table.Column<int>(type: "integer", nullable: false),
                    pi = table.Column<decimal>(type: "numeric", nullable: false),
                    client_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saturation_measurement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saturation_measurement_raw",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    items = table.Column<ICollection<RawSaturationItem>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saturation_measurement_raw", x => x.id);
                    table.ForeignKey(
                        name: "fk_saturation_measurement_raw_saturation_measurement_id",
                        column: x => x.id,
                        principalTable: "saturation_measurement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_blood_pressure_measurement_patient_id_client_date",
                table: "blood_pressure_measurement",
                columns: new[] { "patient_id", "client_date" });

            migrationBuilder.CreateIndex(
                name: "ix_mood_measurement_patient_id_client_date",
                table: "mood_measurement",
                columns: new[] { "patient_id", "client_date" });

            migrationBuilder.CreateIndex(
                name: "ix_pulse_rate_measurement_patient_id_client_date",
                table: "pulse_rate_measurement",
                columns: new[] { "patient_id", "client_date" });

            migrationBuilder.CreateIndex(
                name: "ix_saturation_measurement_patient_id_client_date",
                table: "saturation_measurement",
                columns: new[] { "patient_id", "client_date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blood_pressure_measurement");

            migrationBuilder.DropTable(
                name: "mood_measurement");

            migrationBuilder.DropTable(
                name: "pulse_rate_measurement");

            migrationBuilder.DropTable(
                name: "saturation_measurement_raw");

            migrationBuilder.DropTable(
                name: "saturation_measurement");
        }
    }
}
