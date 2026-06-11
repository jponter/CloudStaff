using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudStaff.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAllocationsToDateRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing rows have null StartDate/EndDate — clear before adding NOT NULL constraint.
            migrationBuilder.Sql("DELETE FROM staff_allocations;");

            migrationBuilder.DropColumn(name: "Month",      table: "staff_allocations");
            migrationBuilder.DropColumn(name: "WeekNumber", table: "staff_allocations");
            migrationBuilder.DropColumn(name: "Year",       table: "staff_allocations");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "staff_allocations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1900, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "staff_allocations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1900, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "staff_allocations",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "staff_allocations",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "staff_allocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeekNumber",
                table: "staff_allocations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "staff_allocations",
                type: "integer",
                nullable: true);
        }
    }
}
