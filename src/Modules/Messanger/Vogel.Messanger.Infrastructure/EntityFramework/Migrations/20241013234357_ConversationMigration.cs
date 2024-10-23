using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Messanger.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ConversationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Messanger");

            migrationBuilder.CreateTable(
                name: "Conversations",
                schema: "Messanger",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                schema: "Messanger",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConversationId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_CreatorId",
                schema: "Messanger",
                table: "Conversations",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_DeletorId",
                schema: "Messanger",
                table: "Conversations",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ModifierId",
                schema: "Messanger",
                table: "Conversations",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_CreatorId",
                schema: "Messanger",
                table: "Participants",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_DeletorId",
                schema: "Messanger",
                table: "Participants",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ModifierId",
                schema: "Messanger",
                table: "Participants",
                column: "ModifierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conversations",
                schema: "Messanger");

            migrationBuilder.DropTable(
                name: "Participants",
                schema: "Messanger");
        }
    }
}
