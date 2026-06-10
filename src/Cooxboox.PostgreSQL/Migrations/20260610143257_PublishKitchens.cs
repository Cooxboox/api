using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class PublishKitchens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublishedBy",
                schema: "Content",
                table: "Kitchens",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedOn",
                schema: "Content",
                table: "Kitchens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PublishedVersion",
                schema: "Content",
                table: "Kitchens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Content",
                table: "Kitchens",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "KitchenLocales",
                schema: "Content",
                columns: table => new
                {
                    KitchenId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
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
                    table.PrimaryKey("PK_KitchenLocales", x => new { x.KitchenId, x.Language });
                    table.ForeignKey(
                        name: "FK_KitchenLocales_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_PublishedBy",
                schema: "Content",
                table: "Kitchens",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_PublishedOn",
                schema: "Content",
                table: "Kitchens",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_PublishedVersion",
                schema: "Content",
                table: "Kitchens",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Status",
                schema: "Content",
                table: "Kitchens",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_CreatedBy",
                schema: "Content",
                table: "KitchenLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_CreatedOn",
                schema: "Content",
                table: "KitchenLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_Language",
                schema: "Content",
                table: "KitchenLocales",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_PublishedBy",
                schema: "Content",
                table: "KitchenLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_PublishedOn",
                schema: "Content",
                table: "KitchenLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_PublishedVersion",
                schema: "Content",
                table: "KitchenLocales",
                column: "PublishedVersion");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_Status",
                schema: "Content",
                table: "KitchenLocales",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_UpdatedBy",
                schema: "Content",
                table: "KitchenLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_UpdatedOn",
                schema: "Content",
                table: "KitchenLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenLocales_Version",
                schema: "Content",
                table: "KitchenLocales",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KitchenLocales",
                schema: "Content");

            migrationBuilder.DropIndex(
                name: "IX_Kitchens_PublishedBy",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropIndex(
                name: "IX_Kitchens_PublishedOn",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropIndex(
                name: "IX_Kitchens_PublishedVersion",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropIndex(
                name: "IX_Kitchens_Status",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropColumn(
                name: "PublishedBy",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropColumn(
                name: "PublishedOn",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropColumn(
                name: "PublishedVersion",
                schema: "Content",
                table: "Kitchens");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Content",
                table: "Kitchens");
        }
    }
}
