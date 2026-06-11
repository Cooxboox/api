using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cooxboox.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateOutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    OutboxMessageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.OutboxMessageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EventId",
                table: "OutboxMessages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Status",
                table: "OutboxMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_StreamId",
                table: "OutboxMessages",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_UpdatedOn",
                table: "OutboxMessages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Version",
                table: "OutboxMessages",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages");
        }
    }
}
