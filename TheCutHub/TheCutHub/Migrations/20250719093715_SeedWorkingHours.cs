using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TheCutHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorkingHours",
                columns: new[] { "Id", "Day", "EndTime", "IsWorking", "StartTime" },
                values: new object[,]
                {
                    { 1, 0, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 2, 1, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 3, 2, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 4, 3, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 5, 4, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 6, 5, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 7, 6, new TimeSpan(0, 18, 0, 0, 0), true, new TimeSpan(0, 9, 0, 0, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
