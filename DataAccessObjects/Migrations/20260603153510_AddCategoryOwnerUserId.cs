using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryOwnerUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_Branch",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14,
                column: "OwnerUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Branch_OwnerUserId",
                table: "Categories",
                columns: new[] { "Name", "Branch", "OwnerUserId" },
                unique: true,
                filter: "[OwnerUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OwnerUserId",
                table: "Categories",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Branch_System",
                table: "Categories",
                columns: new[] { "Name", "Branch" },
                unique: true,
                filter: "[OwnerUserId] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AppUsers_OwnerUserId",
                table: "Categories",
                column: "OwnerUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AppUsers_OwnerUserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_Branch_OwnerUserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_OwnerUserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Branch",
                table: "Categories",
                columns: new[] { "Name", "Branch" },
                unique: true);
        }
    }
}
