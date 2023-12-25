﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UstaYardımAPI.Migrations
{
    /// <inheritdoc />
    public partial class Uptade_Usta_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ustalar_UserId",
                table: "Ustalar",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ustalar_AspNetUsers_UserId",
                table: "Ustalar",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ustalar_AspNetUsers_UserId",
                table: "Ustalar");

            migrationBuilder.DropIndex(
                name: "IX_Ustalar_UserId",
                table: "Ustalar");
        }
    }
}
