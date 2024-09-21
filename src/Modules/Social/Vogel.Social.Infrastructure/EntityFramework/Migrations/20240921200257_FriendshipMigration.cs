using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Social.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FriendshipMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendRequests",
                schema: "Social",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ReciverId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_ReciverId",
                        column: x => x.ReciverId,
                        principalSchema: "Social",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "Social",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                schema: "Social",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TargetId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friends_Users_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "Social",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friends_Users_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "Social",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_CreatorId",
                schema: "Social",
                table: "FriendRequests",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_DeletorId",
                schema: "Social",
                table: "FriendRequests",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ModifierId",
                schema: "Social",
                table: "FriendRequests",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReciverId",
                schema: "Social",
                table: "FriendRequests",
                column: "ReciverId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_SenderId",
                schema: "Social",
                table: "FriendRequests",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_CreatorId",
                schema: "Social",
                table: "Friends",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_DeletorId",
                schema: "Social",
                table: "Friends",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_ModifierId",
                schema: "Social",
                table: "Friends",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_SourceId",
                schema: "Social",
                table: "Friends",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_TargetId",
                schema: "Social",
                table: "Friends",
                column: "TargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequests",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Friends",
                schema: "Social");
        }
    }
}
