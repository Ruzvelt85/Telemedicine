using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemedicine.Services.HealthCenterStructureDomainService.MigrationEfCore.Migrations
{
    public partial class Version_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "health_center",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inner_id = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    usa_state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_health_center", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inner_id = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "interdisciplinary_team",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inner_id = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    health_center_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interdisciplinary_team", x => x.id);
                    table.ForeignKey(
                        name: "fk_interdisciplinary_team_health_center_health_center_id",
                        column: x => x.health_center_id,
                        principalTable: "health_center",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "doctor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor", x => x.id);
                    table.ForeignKey(
                        name: "fk_doctor_user_id",
                        column: x => x.id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "interdisciplinary_team_doctor",
                columns: table => new
                {
                    interdisciplinary_team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interdisciplinary_team_doctor", x => new { x.interdisciplinary_team_id, x.doctor_id });
                    table.ForeignKey(
                        name: "fk_interdisciplinary_team_doctor_interdisciplinary_t",
                        column: x => x.interdisciplinary_team_id,
                        principalTable: "interdisciplinary_team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_interdisciplinary_team_doctor_doctor_me",
                        column: x => x.doctor_id,
                        principalTable: "doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "health_center_doctor",
                columns: table => new
                {
                    health_center_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_health_center_doctor", x => new { x.health_center_id, x.doctor_id });
                    table.ForeignKey(
                        name: "fk_health_center_doctor_doctor_docto",
                        column: x => x.doctor_id,
                        principalTable: "doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_health_center_doctor_health_center_health_center_id",
                        column: x => x.health_center_id,
                        principalTable: "health_center",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    interdisciplinary_team_id = table.Column<Guid>(type: "uuid", nullable: true),
                    health_center_id = table.Column<Guid>(type: "uuid", nullable: false),
                    primary_care_provider_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_interdisciplinary_teams_interdisciplinary_team_",
                        column: x => x.interdisciplinary_team_id,
                        principalTable: "interdisciplinary_team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_patient_doctor_primary_care_provider_id",
                        column: x => x.primary_care_provider_id,
                        principalTable: "doctor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_patient_health_centers_health_center_id",
                        column: x => x.health_center_id,
                        principalTable: "health_center",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_patient_user_id",
                        column: x => x.id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_interdisciplinary_team_inner_id",
                table: "interdisciplinary_team",
                column: "inner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interdisciplinary_team_health_center_id",
                table: "interdisciplinary_team",
                column: "health_center_id");

            migrationBuilder.CreateIndex(
                name: "ix_interdisciplinary_team_doctor_doctor_id",
                table: "interdisciplinary_team_doctor",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "ix_health_center_inner_id",
                table: "health_center",
                column: "inner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_health_center_doctor_doctor_id",
                table: "health_center_doctor",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_interdisciplinary_team_id",
                table: "patient",
                column: "interdisciplinary_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_health_center_id",
                table: "patient",
                column: "health_center_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_primary_care_provider_id",
                table: "patient",
                column: "primary_care_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_inner_id_type",
                table: "user",
                columns: new[] { "inner_id", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_first_name",
                table: "user",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "ix_user_last_name",
                table: "user",
                column: "last_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "interdisciplinary_team_doctor");

            migrationBuilder.DropTable(
                name: "health_center_doctor");

            migrationBuilder.DropTable(
                name: "patient");

            migrationBuilder.DropTable(
                name: "interdisciplinary_team");

            migrationBuilder.DropTable(
                name: "doctor");

            migrationBuilder.DropTable(
                name: "health_center");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
