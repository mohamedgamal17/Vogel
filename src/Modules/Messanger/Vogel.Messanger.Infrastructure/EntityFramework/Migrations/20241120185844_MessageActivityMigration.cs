using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Messanger.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MessageActivityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagesActivites",
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
                    table.PrimaryKey("PK_MessagesActivites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessagesActivites_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Messanger",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessagesActivites_CreatorId",
                schema: "Messanger",
                table: "MessagesActivites",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesActivites_DeletorId",
                schema: "Messanger",
                table: "MessagesActivites",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesActivites_MessageId",
                schema: "Messanger",
                table: "MessagesActivites",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesActivites_ModifierId",
                schema: "Messanger",
                table: "MessagesActivites",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesActivites_SeenById",
                schema: "Messanger",
                table: "MessagesActivites",
                column: "SeenById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagesActivites",
                schema: "Messanger");
        }
    }
}
