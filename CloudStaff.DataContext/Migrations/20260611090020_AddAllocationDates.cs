using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudStaff.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddAllocationDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "staff_allocations",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "staff_allocations",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "staff_allocations");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "staff_allocations");
        }
    }
}
