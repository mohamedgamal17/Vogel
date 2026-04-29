using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Social.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    [Migration("20260428193000_RemovePictureFeature")]
    public partial class RemovePictureFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Pictures_AvatarId",
                schema: "Social",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Pictures",
                schema: "Social");

            migrationBuilder.DropIndex(
                name: "IX_Users_AvatarId",
                schema: "Social",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pictures",
                schema: "Social",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    File = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_CreatorId",
                schema: "Social",
                table: "Pictures",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_DeletorId",
                schema: "Social",
                table: "Pictures",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_ModifierId",
                schema: "Social",
                table: "Pictures",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_UserId",
                schema: "Social",
                table: "Pictures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarId",
                schema: "Social",
                table: "Users",
                column: "AvatarId",
                unique: true,
                filter: "[AvatarId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Pictures_AvatarId",
                schema: "Social",
                table: "Users",
                column: "AvatarId",
                principalSchema: "Social",
                principalTable: "Pictures",
                principalColumn: "Id");
        }
    }
}
