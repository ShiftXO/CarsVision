namespace CarsVision.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ValidityCarProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Validity",
                table: "Cars",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Validity",
                table: "Cars");
        }
    }
}
