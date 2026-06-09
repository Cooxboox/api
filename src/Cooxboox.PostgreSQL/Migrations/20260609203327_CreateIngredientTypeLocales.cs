using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateIngredientTypeLocales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientTypeLocales",
                schema: "Content",
                columns: table => new
                {
                    IngredientTypeId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_IngredientTypeLocales", x => new { x.IngredientTypeId, x.Language });
                    table.ForeignKey(
                        name: "FK_IngredientTypeLocales_IngredientTypes_IngredientTypeId",
                        column: x => x.IngredientTypeId,
                        principalSchema: "Content",
                        principalTable: "IngredientTypes",
                        principalColumn: "IngredientTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientTypeLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_CreatedBy",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_CreatedOn",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_KitchenId_Language_Slug",
                schema: "Content",
                table: "IngredientTypeLocales",
                columns: new[] { "KitchenId", "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_Language",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_Name",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_PublishedBy",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_PublishedOn",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_PublishedVersion",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_Status",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_UpdatedBy",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_UpdatedOn",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeLocales_Version",
                schema: "Content",
                table: "IngredientTypeLocales",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientTypeLocales",
                schema: "Content");
        }
    }
}
