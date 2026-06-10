using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateRecipeTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecipeTypes",
                schema: "Content",
                columns: table => new
                {
                    RecipeTypeId = table.Column<int>(type: "integer", nullable: false)
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
                    table.PrimaryKey("PK_RecipeTypes", x => x.RecipeTypeId);
                    table.ForeignKey(
                        name: "FK_RecipeTypes_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeTypeLocales",
                schema: "Content",
                columns: table => new
                {
                    RecipeTypeId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_RecipeTypeLocales", x => new { x.RecipeTypeId, x.Language });
                    table.ForeignKey(
                        name: "FK_RecipeTypeLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeTypeLocales_RecipeTypes_RecipeTypeId",
                        column: x => x.RecipeTypeId,
                        principalSchema: "Content",
                        principalTable: "RecipeTypes",
                        principalColumn: "RecipeTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_CreatedBy",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_CreatedOn",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_KitchenId_Language_Slug",
                schema: "Content",
                table: "RecipeTypeLocales",
                columns: new[] { "KitchenId", "Language", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_Language",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_Name",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_PublishedBy",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_PublishedOn",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_PublishedVersion",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_Status",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_UpdatedBy",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_UpdatedOn",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypeLocales_Version",
                schema: "Content",
                table: "RecipeTypeLocales",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_CreatedBy",
                schema: "Content",
                table: "RecipeTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_CreatedOn",
                schema: "Content",
                table: "RecipeTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_KitchenId_EntityId",
                schema: "Content",
                table: "RecipeTypes",
                columns: new[] { "KitchenId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_Name",
                schema: "Content",
                table: "RecipeTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_PublishedBy",
                schema: "Content",
                table: "RecipeTypes",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_PublishedOn",
                schema: "Content",
                table: "RecipeTypes",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_PublishedVersion",
                schema: "Content",
                table: "RecipeTypes",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_Status",
                schema: "Content",
                table: "RecipeTypes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_StreamId",
                schema: "Content",
                table: "RecipeTypes",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_UpdatedBy",
                schema: "Content",
                table: "RecipeTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_UpdatedOn",
                schema: "Content",
                table: "RecipeTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeTypes_Version",
                schema: "Content",
                table: "RecipeTypes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeTypeLocales",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "RecipeTypes",
                schema: "Content");
        }
    }
}
