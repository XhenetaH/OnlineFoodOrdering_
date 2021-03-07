using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineFoodOrdering.Data.Migrations
{
    public partial class addUpdatedCompare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Compare_MenuItemId",
                table: "Compare",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compare_MenuItem_MenuItemId",
                table: "Compare",
                column: "MenuItemId",
                principalTable: "MenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compare_MenuItem_MenuItemId",
                table: "Compare");

            migrationBuilder.DropIndex(
                name: "IX_Compare_MenuItemId",
                table: "Compare");
        }
    }
}
