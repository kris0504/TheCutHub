using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCutHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBarberToWorkingHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DELETE FROM [WorkingHours];");

			migrationBuilder.AddColumn<int>(
                name: "BarberId",
                table: "WorkingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 1,
                column: "BarberId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 2,
                column: "BarberId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 3,
                column: "BarberId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 4,
                column: "BarberId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 5,
                column: "BarberId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 6,
                column: "BarberId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 7,
                column: "BarberId",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_BarberId",
                table: "WorkingHours",
                column: "BarberId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingHours_Barbers_BarberId",
                table: "WorkingHours",
                column: "BarberId",
                principalTable: "Barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkingHours_Barbers_BarberId",
                table: "WorkingHours");

            migrationBuilder.DropIndex(
                name: "IX_WorkingHours_BarberId",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "BarberId",
                table: "WorkingHours");
        }
    }
}
