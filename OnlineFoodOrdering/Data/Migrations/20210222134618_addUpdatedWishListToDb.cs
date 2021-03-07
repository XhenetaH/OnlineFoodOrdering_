using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineFoodOrdering.Data.Migrations
{
    public partial class addUpdatedWishListToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "WishList",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "WishList");
        }
    }
}
