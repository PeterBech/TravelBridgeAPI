using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rootobjects",
                columns: table => new
                {
                    Keyword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rootobjects", x => x.Keyword);
                });

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    regionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    countryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    countryNameShort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    photoUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    parent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    region = table.Column<string>(type: "nvarchar(max)", nullable: true), // Ændret til nullable
                    Keyword = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.id);
                    table.ForeignKey(
                        name: "FK_Data_Rootobjects_Keyword",
                        column: x => x.Keyword,
                        principalTable: "Rootobjects",
                        principalColumn: "Keyword",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistancesToCity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<float>(type: "real", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistancesToCity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistancesToCity_Data_DatumId",
                        column: x => x.DatumId,
                        principalTable: "Data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Data_Keyword",
                table: "Data",
                column: "Keyword");

            migrationBuilder.CreateIndex(
                name: "IX_DistancesToCity_DatumId",
                table: "DistancesToCity",
                column: "DatumId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistancesToCity");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Rootobjects");
        }
    }

}
