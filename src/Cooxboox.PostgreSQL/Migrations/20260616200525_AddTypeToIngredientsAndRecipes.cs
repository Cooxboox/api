using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToIngredientsAndRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecipeTypeId",
                schema: "Content",
                table: "Recipes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IngredientTypeId",
                schema: "Content",
                table: "Ingredients",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_RecipeTypeId",
                schema: "Content",
                table: "Recipes",
                column: "RecipeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_IngredientTypeId",
                schema: "Content",
                table: "Ingredients",
                column: "IngredientTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_IngredientTypes_IngredientTypeId",
                schema: "Content",
                table: "Ingredients",
                column: "IngredientTypeId",
                principalSchema: "Content",
                principalTable: "IngredientTypes",
                principalColumn: "IngredientTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeTypes_RecipeTypeId",
                schema: "Content",
                table: "Recipes",
                column: "RecipeTypeId",
                principalSchema: "Content",
                principalTable: "RecipeTypes",
                principalColumn: "RecipeTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_IngredientTypes_IngredientTypeId",
                schema: "Content",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeTypes_RecipeTypeId",
                schema: "Content",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_RecipeTypeId",
                schema: "Content",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_IngredientTypeId",
                schema: "Content",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "RecipeTypeId",
                schema: "Content",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IngredientTypeId",
                schema: "Content",
                table: "Ingredients");
        }
    }
}
