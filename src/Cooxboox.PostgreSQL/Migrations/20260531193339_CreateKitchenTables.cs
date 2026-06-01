using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateKitchenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cooxboox");

            migrationBuilder.CreateTable(
                name: "Kitchens",
                schema: "Cooxboox",
                columns: table => new
                {
                    KitchenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Confidentiality = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchens", x => x.KitchenId);
                });

            migrationBuilder.CreateTable(
                name: "KitchenLocales",
                schema: "Cooxboox",
                columns: table => new
                {
                    KitchenLocaleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KitchenId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    MetaDescription = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_KitchenLocales", x => x.KitchenLocaleId);
                    table.ForeignKey(
                        name: "FK_KitchenLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Cooxboox",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishedKitchenLocales",
                schema: "Cooxboox",
                columns: table => new
                {
                    KitchenLocaleId = table.Column<int>(type: "integer", nullable: false),
                    KitchenId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    MetaDescription = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    HtmlContent = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    PublishedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedKitchenLocales", x => x.KitchenLocaleId);
                    table.ForeignKey(
                        name: "FK_PublishedKitchenLocales_KitchenLocales_KitchenLocaleId",
                        column: x => x.KitchenLocaleId,
                        principalSchema: "Cooxboox",
                        principalTable: "KitchenLocales",
                        principalColumn: "KitchenLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedKitchenLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Cooxboox",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_CreatedBy",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_CreatedOn",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_KitchenId_Language",
                schema: "Cooxboox",
                table: "KitchenLocales",
                columns: new[] { "KitchenId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_Language",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_PublishedBy",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_PublishedOn",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_PublishedVersion",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_Status",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_UpdatedBy",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_UpdatedOn",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_Version",
                schema: "Cooxboox",
                table: "KitchenLocales",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Confidentiality",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "Confidentiality");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_CreatedBy",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_CreatedOn",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Name",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_OwnerId",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Slug",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_StreamId",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_UniqueId",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_UpdatedBy",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_UpdatedOn",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Version",
                schema: "Cooxboox",
                table: "Kitchens",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedKitchenLocales_KitchenId_Language",
                schema: "Cooxboox",
                table: "PublishedKitchenLocales",
                columns: new[] { "KitchenId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublishedKitchenLocales_Language",
                schema: "Cooxboox",
                table: "PublishedKitchenLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedKitchenLocales_PublishedBy",
                schema: "Cooxboox",
                table: "PublishedKitchenLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedKitchenLocales_PublishedOn",
                schema: "Cooxboox",
                table: "PublishedKitchenLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedKitchenLocales_Version",
                schema: "Cooxboox",
                table: "PublishedKitchenLocales",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedKitchenLocales",
                schema: "Cooxboox");

            migrationBuilder.DropTable(
                name: "KitchenLocales",
                schema: "Cooxboox");

            migrationBuilder.DropTable(
                name: "Kitchens",
                schema: "Cooxboox");
        }
    }
}
