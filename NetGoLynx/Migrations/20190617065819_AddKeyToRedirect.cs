using Microsoft.EntityFrameworkCore.Migrations;

namespace NetGoLynx.Migrations
{
    public partial class AddKeyToRedirect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Redirects_Name",
                table: "Redirects",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Redirects_Name",
                table: "Redirects");
        }
    }
}
