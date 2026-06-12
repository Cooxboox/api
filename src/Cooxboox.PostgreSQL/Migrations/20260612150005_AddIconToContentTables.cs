using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddIconToContentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "Content",
                table: "RecipeTypes",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "Content",
                table: "RecipeCategories",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "Content",
                table: "IngredientTypes",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "Content",
                table: "IngredientCategories",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Content",
                table: "RecipeTypes");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Content",
                table: "RecipeCategories");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Content",
                table: "IngredientTypes");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Content",
                table: "IngredientCategories");
        }
    }
}
