using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vogel.Content.Infrastructure.EntityFramework.Migrations
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
                name: "Medias",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    File = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Medias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MediaId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CommentId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "Content",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Content",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostReactions",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReactions_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Content",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentReactions",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CommentId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifierId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletorId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "Content",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId",
                schema: "Content",
                table: "CommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CreatorId",
                schema: "Content",
                table: "CommentReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_DeletorId",
                schema: "Content",
                table: "CommentReactions",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ModifierId",
                schema: "Content",
                table: "CommentReactions",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_UserId",
                schema: "Content",
                table: "CommentReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                schema: "Content",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatorId",
                schema: "Content",
                table: "Comments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DeletorId",
                schema: "Content",
                table: "Comments",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ModifierId",
                schema: "Content",
                table: "Comments",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                schema: "Content",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                schema: "Content",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_CreatorId",
                schema: "Content",
                table: "Medias",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_DeletorId",
                schema: "Content",
                table: "Medias",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_ModifierId",
                schema: "Content",
                table: "Medias",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_UserId",
                schema: "Content",
                table: "Medias",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_CreatorId",
                schema: "Content",
                table: "PostReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_DeletorId",
                schema: "Content",
                table: "PostReactions",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_ModifierId",
                schema: "Content",
                table: "PostReactions",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId",
                schema: "Content",
                table: "PostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_UserId",
                schema: "Content",
                table: "PostReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatorId",
                schema: "Content",
                table: "Posts",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DeletorId",
                schema: "Content",
                table: "Posts",
                column: "DeletorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ModifierId",
                schema: "Content",
                table: "Posts",
                column: "ModifierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentReactions",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Medias",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "PostReactions",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Comments",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "Content");
        }
    }
}
