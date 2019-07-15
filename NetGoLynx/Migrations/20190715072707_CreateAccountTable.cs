using Microsoft.EntityFrameworkCore.Migrations;

namespace NetGoLynx.Migrations
{
#pragma warning disable 1591
    public partial class CreateAccountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Redirects",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Redirects",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Redirects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Redirects_AccountId",
                table: "Redirects",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Name",
                table: "Accounts",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Redirects_Accounts_AccountId",
                table: "Redirects",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Redirects_Accounts_AccountId",
                table: "Redirects");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Redirects_AccountId",
                table: "Redirects");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Redirects");

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Redirects",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Redirects",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255);
        }
    }
#pragma warning restore 1591
}
