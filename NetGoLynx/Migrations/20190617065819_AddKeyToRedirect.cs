using Microsoft.EntityFrameworkCore.Migrations;

namespace NetGoLynx.Migrations
{
#pragma warning disable 1591
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
#pragma warning restore 1591
}
