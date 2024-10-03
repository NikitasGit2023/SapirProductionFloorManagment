using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SapirProductionFloorManagment.Server.Migrations
{
    /// <inheritdoc />
    public partial class Migv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSceduleCalculation",
                table: "AppGeneralData");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWorkPlanCalculation",
                table: "AppGeneralData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastWorkPlanCalculation",
                table: "AppGeneralData");

            migrationBuilder.AddColumn<string>(
                name: "LastSceduleCalculation",
                table: "AppGeneralData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
