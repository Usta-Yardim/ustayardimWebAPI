using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UstaYardımAPI.Migrations
{
    /// <inheritdoc />
    public partial class Kategori_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KategoriId",
                table: "Kategoriler",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "KategoriId",
                table: "Ustalar",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FavoriUstaId",
                table: "Musteriler",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Ustalar_KategoriId",
                table: "Ustalar",
                column: "KategoriId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ustalar_Kategoriler_KategoriId",
                table: "Ustalar",
                column: "KategoriId",
                principalTable: "Kategoriler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ustalar_Kategoriler_KategoriId",
                table: "Ustalar");

            migrationBuilder.DropIndex(
                name: "IX_Ustalar_KategoriId",
                table: "Ustalar");

            migrationBuilder.DropColumn(
                name: "KategoriId",
                table: "Ustalar");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Kategoriler",
                newName: "KategoriId");

            migrationBuilder.AlterColumn<int>(
                name: "FavoriUstaId",
                table: "Musteriler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
