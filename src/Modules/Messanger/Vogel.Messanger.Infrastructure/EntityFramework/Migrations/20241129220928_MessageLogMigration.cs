using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Messanger.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MessageLogMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagesLogs",
                schema: "Messanger",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SeenById = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessagesLogs_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Messanger",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessagesLogs_CreatorId",
                schema: "Messanger",
                table: "MessagesLogs",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesLogs_DeletorId",
                schema: "Messanger",
                table: "MessagesLogs",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesLogs_MessageId",
                schema: "Messanger",
                table: "MessagesLogs",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesLogs_ModifierId",
                schema: "Messanger",
                table: "MessagesLogs",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesLogs_SeenById",
                schema: "Messanger",
                table: "MessagesLogs",
                column: "SeenById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagesLogs",
                schema: "Messanger");
        }
    }
}
