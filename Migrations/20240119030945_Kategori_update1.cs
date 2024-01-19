using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UstaYardımAPI.Migrations
{
    /// <inheritdoc />
    public partial class Kategori_update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Puan",
                table: "Ustalar",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KategoriUrl",
                table: "Kategoriler",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KategoriUrl",
                table: "Kategoriler");

            migrationBuilder.AlterColumn<int>(
                name: "Puan",
                table: "Ustalar",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);
        }
    }
}
