using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateRecipeCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecipeCategories",
                schema: "Content",
                columns: table => new
                {
                    RecipeCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KitchenId = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    PublishedVersion = table.Column<long>(type: "bigint", nullable: true),
                    PublishedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeCategories", x => x.RecipeCategoryId);
                    table.ForeignKey(
                        name: "FK_RecipeCategories_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeCategoryLocales",
                schema: "Content",
                columns: table => new
                {
                    RecipeCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    KitchenId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MetaDescription = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    PublishedVersion = table.Column<long>(type: "bigint", nullable: true),
                    PublishedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeCategoryLocales", x => new { x.RecipeCategoryId, x.Language });
                    table.ForeignKey(
                        name: "FK_RecipeCategoryLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeCategoryLocales_RecipeCategories_RecipeCategoryId",
                        column: x => x.RecipeCategoryId,
                        principalSchema: "Content",
                        principalTable: "RecipeCategories",
                        principalColumn: "RecipeCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_CreatedBy",
                schema: "Content",
                table: "RecipeCategories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_CreatedOn",
                schema: "Content",
                table: "RecipeCategories",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_KitchenId_EntityId",
                schema: "Content",
                table: "RecipeCategories",
                columns: new[] { "KitchenId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_Name",
                schema: "Content",
                table: "RecipeCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_PublishedBy",
                schema: "Content",
                table: "RecipeCategories",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_PublishedOn",
                schema: "Content",
                table: "RecipeCategories",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_PublishedVersion",
                schema: "Content",
                table: "RecipeCategories",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_Status",
                schema: "Content",
                table: "RecipeCategories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_StreamId",
                schema: "Content",
                table: "RecipeCategories",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_UpdatedBy",
                schema: "Content",
                table: "RecipeCategories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_UpdatedOn",
                schema: "Content",
                table: "RecipeCategories",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategories_Version",
                schema: "Content",
                table: "RecipeCategories",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_CreatedBy",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_CreatedOn",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_KitchenId_Language_Slug",
                schema: "Content",
                table: "RecipeCategoryLocales",
                columns: new[] { "KitchenId", "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_Language",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_Name",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_PublishedBy",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_PublishedOn",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_PublishedVersion",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_Status",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_UpdatedBy",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_UpdatedOn",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategoryLocales_Version",
                schema: "Content",
                table: "RecipeCategoryLocales",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeCategoryLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "RecipeCategories",
                schema: "Content");
        }
    }
}
