using Microsoft.EntityFrameworkCore.Migrations;

namespace NetGoLynx.Migrations
{
#pragma warning disable 1591
    public partial class AddDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Redirects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Redirects");
        }
    }
#pragma warning restore 1591
}
