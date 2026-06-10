using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class PublishIngredientTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublishedBy",
                schema: "Content",
                table: "IngredientTypes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedOn",
                schema: "Content",
                table: "IngredientTypes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PublishedVersion",
                schema: "Content",
                table: "IngredientTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Content",
                table: "IngredientTypes",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_PublishedBy",
                schema: "Content",
                table: "IngredientTypes",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_PublishedOn",
                schema: "Content",
                table: "IngredientTypes",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_PublishedVersion",
                schema: "Content",
                table: "IngredientTypes",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_Status",
                schema: "Content",
                table: "IngredientTypes",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IngredientTypes_PublishedBy",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropIndex(
                name: "IX_IngredientTypes_PublishedOn",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropIndex(
                name: "IX_IngredientTypes_PublishedVersion",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropIndex(
                name: "IX_IngredientTypes_Status",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropColumn(
                name: "PublishedBy",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropColumn(
                name: "PublishedOn",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropColumn(
                name: "PublishedVersion",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Content",
                table: "IngredientTypes");
        }
    }
}
