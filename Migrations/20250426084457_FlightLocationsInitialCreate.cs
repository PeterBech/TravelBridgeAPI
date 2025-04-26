using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBridgeAPI.Migrations
{
    /// <inheritdoc />
    public partial class FlightLocationsInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rootobjects",
                columns: table => new
                {
                    Keyword = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<bool>(type: "INTEGER", nullable: false),
                    message = table.Column<string>(type: "TEXT", nullable: false),
                    timestamp = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rootobjects", x => new { x.Keyword, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    DataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    code = table.Column<string>(type: "TEXT", nullable: true),
                    city = table.Column<string>(type: "TEXT", nullable: true),
                    cityName = table.Column<string>(type: "TEXT", nullable: true),
                    regionName = table.Column<string>(type: "TEXT", nullable: true),
                    country = table.Column<string>(type: "TEXT", nullable: true),
                    countryName = table.Column<string>(type: "TEXT", nullable: true),
                    countryNameShort = table.Column<string>(type: "TEXT", nullable: true),
                    photoUri = table.Column<string>(type: "TEXT", nullable: true),
                    parent = table.Column<string>(type: "TEXT", nullable: true),
                    region = table.Column<string>(type: "TEXT", nullable: true),
                    Keyword = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.DataId);
                    table.ForeignKey(
                        name: "FK_Data_Rootobjects_Keyword_Language",
                        columns: x => new { x.Keyword, x.Language },
                        principalTable: "Rootobjects",
                        principalColumns: new[] { "Keyword", "Language" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistancesToCity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    value = table.Column<float>(type: "REAL", nullable: true),
                    unit = table.Column<string>(type: "TEXT", nullable: true),
                    DatumId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistancesToCity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistancesToCity_Data_DatumId",
                        column: x => x.DatumId,
                        principalTable: "Data",
                        principalColumn: "DataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Data_Keyword_Language",
                table: "Data",
                columns: new[] { "Keyword", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_DistancesToCity_DatumId",
                table: "DistancesToCity",
                column: "DatumId",
                unique: true);
        }

        /// <inheritdoc />
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
