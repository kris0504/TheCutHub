using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCutHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToBarber_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Barbers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Barbers_UserId",
                table: "Barbers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barbers_AspNetUsers_UserId",
                table: "Barbers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barbers_AspNetUsers_UserId",
                table: "Barbers");

            migrationBuilder.DropIndex(
                name: "IX_Barbers_UserId",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Barbers");
        }
    }
}
