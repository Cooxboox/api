using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ingredients",
                schema: "Content",
                columns: table => new
                {
                    IngredientId = table.Column<int>(type: "integer", nullable: false)
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
                    table.PrimaryKey("PK_Ingredients", x => x.IngredientId);
                    table.ForeignKey(
                        name: "FK_Ingredients_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredientLocales",
                schema: "Content",
                columns: table => new
                {
                    IngredientId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_IngredientLocales", x => new { x.IngredientId, x.Language });
                    table.ForeignKey(
                        name: "FK_IngredientLocales_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "Content",
                        principalTable: "Ingredients",
                        principalColumn: "IngredientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_CreatedBy",
                schema: "Content",
                table: "IngredientLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_CreatedOn",
                schema: "Content",
                table: "IngredientLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_KitchenId_Language_Slug",
                schema: "Content",
                table: "IngredientLocales",
                columns: new[] { "KitchenId", "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_Language",
                schema: "Content",
                table: "IngredientLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_Name",
                schema: "Content",
                table: "IngredientLocales",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_PublishedBy",
                schema: "Content",
                table: "IngredientLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_PublishedOn",
                schema: "Content",
                table: "IngredientLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_PublishedVersion",
                schema: "Content",
                table: "IngredientLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_Status",
                schema: "Content",
                table: "IngredientLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_UpdatedBy",
                schema: "Content",
                table: "IngredientLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_UpdatedOn",
                schema: "Content",
                table: "IngredientLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientLocales_Version",
                schema: "Content",
                table: "IngredientLocales",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CreatedBy",
                schema: "Content",
                table: "Ingredients",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CreatedOn",
                schema: "Content",
                table: "Ingredients",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_KitchenId_EntityId",
                schema: "Content",
                table: "Ingredients",
                columns: new[] { "KitchenId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Name",
                schema: "Content",
                table: "Ingredients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_PublishedBy",
                schema: "Content",
                table: "Ingredients",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_PublishedOn",
                schema: "Content",
                table: "Ingredients",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_PublishedVersion",
                schema: "Content",
                table: "Ingredients",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Status",
                schema: "Content",
                table: "Ingredients",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_StreamId",
                schema: "Content",
                table: "Ingredients",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_UpdatedBy",
                schema: "Content",
                table: "Ingredients",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_UpdatedOn",
                schema: "Content",
                table: "Ingredients",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Version",
                schema: "Content",
                table: "Ingredients",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Ingredients",
                schema: "Content");
        }
    }
}
