using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineFoodOrdering.Data.Migrations
{
    public partial class RegisterDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "AspNetUsers",
                nullable: true);
        }

        
    }
}
