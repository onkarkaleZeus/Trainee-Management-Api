using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserFKSubmissionFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SubmissionFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_UserId",
                table: "SubmissionFiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFiles_Users_UserId",
                table: "SubmissionFiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFiles_Users_UserId",
                table: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionFiles_UserId",
                table: "SubmissionFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SubmissionFiles");
        }
    }
}
