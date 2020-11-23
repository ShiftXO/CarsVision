namespace CarsVision.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddRemotePictureUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemotePictureUrl",
                table: "Pictures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemotePictureUrl",
                table: "Pictures");
        }
    }
}
