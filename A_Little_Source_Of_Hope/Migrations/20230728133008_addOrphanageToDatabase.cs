using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A_Little_Source_Of_Hope.Migrations
{
    /// <inheritdoc />
    public partial class addOrphanageToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orphanage",
                columns: table => new
                {
                    OrphanageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrphanageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrphanageEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrphanageAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CellNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orphanage", x => x.OrphanageId);
                    table.ForeignKey(
                        name: "FK_Orphanage_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    provinceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.provinceId);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    cityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    provinceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.cityId);
                    table.ForeignKey(
                        name: "FK_City_Province_provinceId",
                        column: x => x.provinceId,
                        principalTable: "Province",
                        principalColumn: "provinceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_City_provinceId",
                table: "City",
                column: "provinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orphanage_AppUserId",
                table: "Orphanage",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Orphanage");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
