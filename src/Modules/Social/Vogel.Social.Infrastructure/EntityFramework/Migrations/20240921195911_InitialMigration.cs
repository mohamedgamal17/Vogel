using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Social.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Social");

            migrationBuilder.CreateTable(
                name: "Pictures",
                schema: "Social",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    File = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Social",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    AvatarId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Pictures_AvatarId",
                        column: x => x.AvatarId,
                        principalSchema: "Social",
                        principalTable: "Pictures",
                        principalColumn: "Id");
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

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatorId",
                schema: "Social",
                table: "Users",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletorId",
                schema: "Social",
                table: "Users",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifierId",
                schema: "Social",
                table: "Users",
                column: "ModifierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Pictures",
                schema: "Social");
        }
    }
}
