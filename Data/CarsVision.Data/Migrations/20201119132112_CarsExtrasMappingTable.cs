namespace CarsVision.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CarsExtrasMappingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Extras_Cars_CarId",
                table: "Extras");

            migrationBuilder.DropIndex(
                name: "IX_Extras_CarId",
                table: "Extras");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "Extras");

            migrationBuilder.CreateTable(
                name: "CarsExtras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(nullable: false),
                    ExtraId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarsExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarsExtras_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarsExtras_Extras_ExtraId",
                        column: x => x.ExtraId,
                        principalTable: "Extras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarsExtras_CarId",
                table: "CarsExtras",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarsExtras_ExtraId",
                table: "CarsExtras",
                column: "ExtraId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarsExtras");

            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "Extras",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Extras_CarId",
                table: "Extras",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Extras_Cars_CarId",
                table: "Extras",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
