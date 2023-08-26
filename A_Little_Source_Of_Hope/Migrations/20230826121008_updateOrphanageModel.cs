using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A_Little_Source_Of_Hope.Migrations
{
    /// <inheritdoc />
    public partial class updateOrphanageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_AppUserId",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "Manager",
                table: "Orphanage");

            migrationBuilder.DropColumn(
                name: "OrphanageEmail",
                table: "Orphanage");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "ShoppingCart",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Imageurl",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_AppUserId",
                table: "ShoppingCart",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_AppUserId",
                table: "ShoppingCart");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "ShoppingCart",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Imageurl",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manager",
                table: "Orphanage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrphanageEmail",
                table: "Orphanage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_AppUserId",
                table: "ShoppingCart",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
