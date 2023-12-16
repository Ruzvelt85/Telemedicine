using Microsoft.EntityFrameworkCore.Migrations;

namespace Telemedicine.Services.HealthCenterStructureDomainService.MigrationEfCore.Migrations
{
    public partial class JD1475_AddAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_admin",
                table: "doctor",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_admin",
                table: "doctor");
        }
    }
}
