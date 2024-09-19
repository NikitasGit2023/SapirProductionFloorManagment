using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SapirProductionFloorManagment.Server.Migrations
{
    /// <inheritdoc />
    public partial class MainDbContextMigration_V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppGeneralData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastSceduleCalculation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGeneralData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    LineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionRate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.LineId);
                });

            migrationBuilder.CreateTable(
                name: "LinesWorkSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RelatedToLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartWork = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndWork = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkDuraion = table.Column<double>(type: "float", nullable: false),
                    FormatedWorkDuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuantityInKg = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkOrderSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeToFinish = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SizeInMicron = table.Column<int>(type: "int", nullable: false),
                    IsCalculted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinesWorkSchedule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinseWorkHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferencedToLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkDay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftStartWork = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftEndWork = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreakStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreakEnd = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinseWorkHours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSN = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrdersFromXL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OptionalLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionalLine2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SizeInMicron = table.Column<int>(type: "int", nullable: false),
                    ProducedQuantity = table.Column<int>(type: "int", nullable: false),
                    QuantityLeft = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrdersFromXL", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppGeneralData");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "LinesWorkSchedule");

            migrationBuilder.DropTable(
                name: "LinseWorkHours");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkOrdersFromXL");
        }
    }
}
