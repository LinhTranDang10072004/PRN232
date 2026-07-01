using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class BudgetWalletAndCarryOver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CarryOverDebt",
                table: "BudgetDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ForwardedOverflow",
                table: "BudgetDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "WalletId",
                table: "Budget",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budget_WalletId",
                table: "Budget",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Wallet_WalletId",
                table: "Budget",
                column: "WalletId",
                principalTable: "Wallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Wallet_WalletId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_WalletId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "CarryOverDebt",
                table: "BudgetDetail");

            migrationBuilder.DropColumn(
                name: "ForwardedOverflow",
                table: "BudgetDetail");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "Budget");
        }
    }
}
