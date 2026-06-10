using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateIngredientCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientCategories",
                schema: "Content",
                columns: table => new
                {
                    IngredientCategoryId = table.Column<int>(type: "integer", nullable: false)
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
                    table.PrimaryKey("PK_IngredientCategories", x => x.IngredientCategoryId);
                    table.ForeignKey(
                        name: "FK_IngredientCategories_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredientCategoryLocales",
                schema: "Content",
                columns: table => new
                {
                    IngredientCategoryId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_IngredientCategoryLocales", x => new { x.IngredientCategoryId, x.Language });
                    table.ForeignKey(
                        name: "FK_IngredientCategoryLocales_IngredientCategories_IngredientCa~",
                        column: x => x.IngredientCategoryId,
                        principalSchema: "Content",
                        principalTable: "IngredientCategories",
                        principalColumn: "IngredientCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientCategoryLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_CreatedBy",
                schema: "Content",
                table: "IngredientCategories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_CreatedOn",
                schema: "Content",
                table: "IngredientCategories",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_KitchenId_EntityId",
                schema: "Content",
                table: "IngredientCategories",
                columns: new[] { "KitchenId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_Name",
                schema: "Content",
                table: "IngredientCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_PublishedBy",
                schema: "Content",
                table: "IngredientCategories",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_PublishedOn",
                schema: "Content",
                table: "IngredientCategories",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_PublishedVersion",
                schema: "Content",
                table: "IngredientCategories",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_Status",
                schema: "Content",
                table: "IngredientCategories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_StreamId",
                schema: "Content",
                table: "IngredientCategories",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_UpdatedBy",
                schema: "Content",
                table: "IngredientCategories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_UpdatedOn",
                schema: "Content",
                table: "IngredientCategories",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_Version",
                schema: "Content",
                table: "IngredientCategories",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_CreatedBy",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_CreatedOn",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_KitchenId_Language_Slug",
                schema: "Content",
                table: "IngredientCategoryLocales",
                columns: new[] { "KitchenId", "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_Language",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_Name",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_PublishedBy",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_PublishedOn",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_PublishedVersion",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_Status",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_UpdatedBy",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_UpdatedOn",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLocales_Version",
                schema: "Content",
                table: "IngredientCategoryLocales",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientCategoryLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "IngredientCategories",
                schema: "Content");
        }
    }
}
