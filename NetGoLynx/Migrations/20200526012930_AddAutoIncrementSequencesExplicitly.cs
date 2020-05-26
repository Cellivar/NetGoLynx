using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetGoLynx.Migrations
{
    public partial class AddAutoIncrementSequencesExplicitly : Migration
    {
        private const string SqlServerProvider = "Microsoft.EntityFrameworkCore.SqlServer";
        private const string SqliteProvider = "Microsoft.EntityFrameworkCore.Sqlite";
        private const string PostgresqlProvider = "Npgsql.EntityFrameworkCore.PostgreSQL";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == SqliteProvider)
            {
                // Sqlite doesn't support sequences and handles this on keys automatically. Safe to just skip.
                return;
            }

            migrationBuilder.CreateSequence<int>(
                name: "AccountNumbers");

            migrationBuilder.CreateSequence<int>(
                name: "RedirectNumbers");


            migrationBuilder.AlterColumn<int>(
                name: "RedirectId",
                table: "Redirects",
                nullable: false,
                defaultValueSql: GetSqlSyntax(migrationBuilder.ActiveProvider, "RedirectNumbers"),
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "Accounts",
                nullable: false,
                defaultValueSql: GetSqlSyntax(migrationBuilder.ActiveProvider, "AccountNumbers"),
                oldClrType: typeof(int));

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == SqliteProvider)
            {
                // Sqlite doesn't support sequences and handles this on keys automatically. Safe to just skip.
                return;
            }
            {
                migrationBuilder.DropSequence(
                name: "AccountNumbers");

                migrationBuilder.DropSequence(
                    name: "RedirectNumbers");

                migrationBuilder.AlterColumn<int>(
                    name: "RedirectId",
                    table: "Redirects",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldDefaultValueSql: GetSqlSyntax(migrationBuilder.ActiveProvider, "RedirectNumbers"));

                migrationBuilder.AlterColumn<int>(
                    name: "AccountId",
                    table: "Accounts",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldDefaultValueSql: GetSqlSyntax(migrationBuilder.ActiveProvider, "AccountNumbers"));
            }
        }

        /// <summary>
        /// Gets the appropriate SQL syntax for a specific provider
        /// </summary>
        /// <remarks>
        /// Because what are standards even.
        /// </remarks>
        /// <param name="provider">The EntityFramework provider the migration is being applied to.</param>
        /// <param name="sequenceName">The sequence to get the next value for.</param>
        /// <returns>A properly formatted SQL operation to get the next value of a sequence.</returns>
        private string GetSqlSyntax(string provider, string sequenceName)
        {
            return provider switch
            {
                SqlServerProvider => $"NEXT VALUE FOR {sequenceName}",
                PostgresqlProvider => $"nextval('\"{sequenceName}\"')",
                _ => throw new ArgumentException($"Unknown provider '{provider}', can't generate sql sequence syntax."),
            };
        }
    }
}
