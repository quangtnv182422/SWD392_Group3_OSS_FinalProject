using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingSystem_Main.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "Province",
                table: "AspNetUsers");


            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                maxLength: 100,
                nullable: true);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

         


            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

        }
    }
}
