using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UstaYardımAPI.Migrations
{
    /// <inheritdoc />
    public partial class delete_usta_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Ustalar",
                table: "Ustalar");

            migrationBuilder.DropIndex(
                name: "IX_Ustalar_UserId",
                table: "Ustalar");

            migrationBuilder.DropColumn(
                name: "UstaId",
                table: "Ustalar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ustalar",
                table: "Ustalar",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Ustalar",
                table: "Ustalar");

            migrationBuilder.AddColumn<int>(
                name: "UstaId",
                table: "Ustalar",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ustalar",
                table: "Ustalar",
                columns: new[] { "UstaId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ustalar_UserId",
                table: "Ustalar",
                column: "UserId");
        }
    }
}
