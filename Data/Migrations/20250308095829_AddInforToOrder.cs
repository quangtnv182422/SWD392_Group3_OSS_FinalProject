using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingSystem_Main.Migrations
{
	/// <inheritdoc />
	public partial class AddInforToOrder : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{

			migrationBuilder.AddColumn<string>(
				name: "FullName",
				table: "Order",
				type: "nvarchar(255)", // Cho phép viết tiếng Việt (Unicode)
				maxLength: 255,
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "PhoneNumber",
				table: "Order",
				type: "nvarchar(20)", // Đủ để chứa số điện thoại
				maxLength: 20,
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Email",
				table: "Order",
				type: "nvarchar(255)", // Email có thể dài nhưng thường không quá 255 ký tự
				maxLength: 255,
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "FullName",
				table: "Order");

			migrationBuilder.DropColumn(
				name: "PhoneNumber",
				table: "Order");

			migrationBuilder.DropColumn(
				name: "Email",
				table: "Order");
		}
	}
}
