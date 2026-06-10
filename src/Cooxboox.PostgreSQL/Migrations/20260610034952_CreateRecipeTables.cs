using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateRecipeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                schema: "Content",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "integer", nullable: false)
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
                    table.PrimaryKey("PK_Recipes", x => x.RecipeId);
                    table.ForeignKey(
                        name: "FK_Recipes_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeLocales",
                schema: "Content",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_RecipeLocales", x => new { x.RecipeId, x.Language });
                    table.ForeignKey(
                        name: "FK_RecipeLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeLocales_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "Content",
                        principalTable: "Recipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_CreatedBy",
                schema: "Content",
                table: "RecipeLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_CreatedOn",
                schema: "Content",
                table: "RecipeLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_KitchenId_Language_Slug",
                schema: "Content",
                table: "RecipeLocales",
                columns: new[] { "KitchenId", "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_Language",
                schema: "Content",
                table: "RecipeLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_Name",
                schema: "Content",
                table: "RecipeLocales",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_PublishedBy",
                schema: "Content",
                table: "RecipeLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_PublishedOn",
                schema: "Content",
                table: "RecipeLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_PublishedVersion",
                schema: "Content",
                table: "RecipeLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_Status",
                schema: "Content",
                table: "RecipeLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_UpdatedBy",
                schema: "Content",
                table: "RecipeLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_UpdatedOn",
                schema: "Content",
                table: "RecipeLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeLocales_Version",
                schema: "Content",
                table: "RecipeLocales",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedBy",
                schema: "Content",
                table: "Recipes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedOn",
                schema: "Content",
                table: "Recipes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_KitchenId_EntityId",
                schema: "Content",
                table: "Recipes",
                columns: new[] { "KitchenId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Name",
                schema: "Content",
                table: "Recipes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_PublishedBy",
                schema: "Content",
                table: "Recipes",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_PublishedOn",
                schema: "Content",
                table: "Recipes",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_PublishedVersion",
                schema: "Content",
                table: "Recipes",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Status",
                schema: "Content",
                table: "Recipes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_StreamId",
                schema: "Content",
                table: "Recipes",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_UpdatedBy",
                schema: "Content",
                table: "Recipes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_UpdatedOn",
                schema: "Content",
                table: "Recipes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Version",
                schema: "Content",
                table: "Recipes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Recipes",
                schema: "Content");
        }
    }
}
