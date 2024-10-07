using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SapirProductionFloorManagment.Server.Migrations
{
    /// <inheritdoc />
    public partial class Migv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
             name: "FullName",
             table: "Users",
             newName: "UserName");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
