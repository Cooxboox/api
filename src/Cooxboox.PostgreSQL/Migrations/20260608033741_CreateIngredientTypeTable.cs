using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateIngredientTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientTypes",
                schema: "Content",
                columns: table => new
                {
                    IngredientTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KitchenId = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientTypes", x => x.IngredientTypeId);
                    table.ForeignKey(
                        name: "FK_IngredientTypes_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalSchema: "Content",
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_CreatedBy",
                schema: "Content",
                table: "IngredientTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_CreatedOn",
                schema: "Content",
                table: "IngredientTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_KitchenId_EntityId",
                schema: "Content",
                table: "IngredientTypes",
                columns: new[] { "KitchenId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_Name",
                schema: "Content",
                table: "IngredientTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_StreamId",
                schema: "Content",
                table: "IngredientTypes",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_UpdatedBy",
                schema: "Content",
                table: "IngredientTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_UpdatedOn",
                schema: "Content",
                table: "IngredientTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypes_Version",
                schema: "Content",
                table: "IngredientTypes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientTypes",
                schema: "Content");
        }
    }
}
