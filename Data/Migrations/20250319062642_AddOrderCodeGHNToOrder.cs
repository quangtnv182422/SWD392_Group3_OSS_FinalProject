using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingSystem_Main.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCodeGHNToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Product__Categor__7C4F7684",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK__Product__Product__7D439ABD",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "ProductStatusId",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "OrderCodeGHN",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Product__Categor__7C4F7684",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK__Product__Product__7D439ABD",
                table: "Product",
                column: "ProductStatusId",
                principalTable: "ProductStatus",
                principalColumn: "ProductStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Product__Categor__7C4F7684",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK__Product__Product__7D439ABD",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "OrderCodeGHN",
                table: "Order");

            migrationBuilder.AlterColumn<int>(
                name: "ProductStatusId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__Product__Categor__7C4F7684",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Product__Product__7D439ABD",
                table: "Product",
                column: "ProductStatusId",
                principalTable: "ProductStatus",
                principalColumn: "ProductStatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
