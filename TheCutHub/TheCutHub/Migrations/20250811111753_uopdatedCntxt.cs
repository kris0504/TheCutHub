using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TheCutHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class uopdatedCntxt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkingHours_BarberId",
                table: "WorkingHours");

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "WorkingHours",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "SlotIntervalInMinutes",
                table: "WorkingHours",
                type: "int",
                nullable: false,
                defaultValue: 30,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "DurationMinutes", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Classic haircut", 30, "Haircut", 25m },
                    { 2, "Beard shaping", 20, "Beard Trim", 15m },
                    { 3, "Haircut + beard", 45, "Combo", 35m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_BarberId_Day",
                table: "WorkingHours",
                columns: new[] { "BarberId", "Day" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkingHours_BarberId_Day",
                table: "WorkingHours");

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "SlotIntervalInMinutes",
                table: "WorkingHours",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "WorkingHours",
                columns: new[] { "Id", "BarberId", "Day", "EndTime", "IsWorking", "SlotIntervalInMinutes", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, 0, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) },
                    { 2, 1, 1, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) },
                    { 3, 1, 2, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) },
                    { 4, 1, 3, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) },
                    { 5, 1, 4, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) },
                    { 6, 1, 5, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) },
                    { 7, 1, 6, new TimeSpan(0, 18, 0, 0, 0), true, 30, new TimeSpan(0, 9, 0, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_BarberId",
                table: "WorkingHours",
                column: "BarberId");
        }
    }
}
