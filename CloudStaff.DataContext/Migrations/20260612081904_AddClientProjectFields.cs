using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudStaff.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddClientProjectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountManagerEmail",
                table: "clients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountManagerName",
                table: "clients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutiveSponsorEmail",
                table: "clients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutiveSponsorName",
                table: "clients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContactEmail",
                table: "clients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContactName",
                table: "clients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "client_projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectManagerEmail",
                table: "client_projects",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectManagerName",
                table: "client_projects",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "client_projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "client_projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountManagerEmail",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "AccountManagerName",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "ExecutiveSponsorEmail",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "ExecutiveSponsorName",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "PrimaryContactEmail",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "PrimaryContactName",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "client_projects");

            migrationBuilder.DropColumn(
                name: "ProjectManagerEmail",
                table: "client_projects");

            migrationBuilder.DropColumn(
                name: "ProjectManagerName",
                table: "client_projects");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "client_projects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "client_projects");
        }
    }
}
