using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingSystem_Main.Migrations
{
	/// <inheritdoc />
	public partial class AddTotalCostToOrder : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<float>(
				name: "TotalCost",
				table: "Order",
				type: "real", // 'real' tương đương float trong SQL Server
				nullable: false,
				defaultValue: 0f);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "TotalCost",
				table: "Order");
		}
	}
}
