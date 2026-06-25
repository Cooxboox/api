using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Content");

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    HistoryRecordId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    KitchenId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResourceKind = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EventData = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.HistoryRecordId);
                });

            migrationBuilder.CreateTable(
                name: "Kitchens",
                schema: "Content",
                columns: table => new
                {
                    KitchenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Confidentiality = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    PublishedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchens", x => x.KitchenId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_EventId",
                table: "History",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_EventType",
                table: "History",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_History_KitchenId_ResourceKind_ResourceId",
                table: "History",
                columns: new[] { "KitchenId", "ResourceKind", "ResourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_History_OccurredOn",
                table: "History",
                column: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_History_UserId",
                table: "History",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_History_Version",
                table: "History",
                column: "Version");

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
                name: "IX_Kitchens_Id",
                schema: "Content",
                table: "Kitchens",
                column: "Id",
                unique: true);

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
                name: "IX_Kitchens_Slug",
                schema: "Content",
                table: "Kitchens",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_Status",
                schema: "Content",
                table: "Kitchens",
                column: "Status");

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
                name: "History");

            migrationBuilder.DropTable(
                name: "Kitchens",
                schema: "Content");
        }
    }
}
