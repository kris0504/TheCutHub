using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheCutHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class timespanToMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Services");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Services");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Services",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
