using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateKitchenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Content");

            migrationBuilder.CreateTable(
                name: "Kitchens",
                schema: "Content",
                columns: table => new
                {
                    KitchenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Confidentiality = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Confidentiality",
                schema: "Content",
                table: "Kitchens",
                column: "Confidentiality");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_CreatedBy",
                schema: "Content",
                table: "Kitchens",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_CreatedOn",
                schema: "Content",
                table: "Kitchens",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Name",
                schema: "Content",
                table: "Kitchens",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_OwnerId",
                schema: "Content",
                table: "Kitchens",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Slug",
                schema: "Content",
                table: "Kitchens",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_StreamId",
                schema: "Content",
                table: "Kitchens",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_UniqueId",
                schema: "Content",
                table: "Kitchens",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_UpdatedBy",
                schema: "Content",
                table: "Kitchens",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_UpdatedOn",
                schema: "Content",
                table: "Kitchens",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Version",
                schema: "Content",
                table: "Kitchens",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kitchens",
                schema: "Content");
        }
    }
}
