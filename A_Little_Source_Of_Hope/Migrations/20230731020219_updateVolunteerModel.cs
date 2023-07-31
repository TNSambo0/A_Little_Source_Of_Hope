using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A_Little_Source_Of_Hope.Migrations
{
    /// <inheritdoc />
    public partial class updateVolunteerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrphanageId",
                table: "Volunteer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Volunteer_OrphanageId",
                table: "Volunteer",
                column: "OrphanageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Volunteer_Orphanage_OrphanageId",
                table: "Volunteer",
                column: "OrphanageId",
                principalTable: "Orphanage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Volunteer_Orphanage_OrphanageId",
                table: "Volunteer");

            migrationBuilder.DropIndex(
                name: "IX_Volunteer_OrphanageId",
                table: "Volunteer");

            migrationBuilder.DropColumn(
                name: "OrphanageId",
                table: "Volunteer");
        }
    }
}
