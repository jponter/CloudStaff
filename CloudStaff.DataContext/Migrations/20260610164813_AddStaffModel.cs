using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CloudStaff.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AsNumber",
                table: "staff",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContractJobTitle",
                table: "staff",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GcmLevel",
                table: "staff",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomePoolId",
                table: "staff",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SfiaLevel",
                table: "staff",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffRoleId",
                table: "staff",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "platforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staff_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "staff_platforms",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "integer", nullable: false),
                    PlatformId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_platforms", x => new { x.StaffId, x.PlatformId });
                    table.ForeignKey(
                        name: "FK_staff_platforms_platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_staff_platforms_staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "home_pools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_home_pools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_home_pools_staff_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "staff_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff_roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HomePoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_staff_roles_home_pools_HomePoolId",
                        column: x => x.HomePoolId,
                        principalTable: "home_pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_staff_AsNumber",
                table: "staff",
                column: "AsNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_staff_HomePoolId",
                table: "staff",
                column: "HomePoolId");

            migrationBuilder.CreateIndex(
                name: "IX_staff_StaffRoleId",
                table: "staff",
                column: "StaffRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_home_pools_CategoryId",
                table: "home_pools",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_home_pools_Code",
                table: "home_pools",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platforms_Name",
                table: "platforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_staff_platforms_PlatformId",
                table: "staff_platforms",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_staff_roles_HomePoolId",
                table: "staff_roles",
                column: "HomePoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_staff_home_pools_HomePoolId",
                table: "staff",
                column: "HomePoolId",
                principalTable: "home_pools",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_staff_staff_roles_StaffRoleId",
                table: "staff",
                column: "StaffRoleId",
                principalTable: "staff_roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_home_pools_HomePoolId",
                table: "staff");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_staff_roles_StaffRoleId",
                table: "staff");

            migrationBuilder.DropTable(
                name: "staff_platforms");

            migrationBuilder.DropTable(
                name: "staff_roles");

            migrationBuilder.DropTable(
                name: "platforms");

            migrationBuilder.DropTable(
                name: "home_pools");

            migrationBuilder.DropTable(
                name: "staff_categories");

            migrationBuilder.DropIndex(
                name: "IX_staff_AsNumber",
                table: "staff");

            migrationBuilder.DropIndex(
                name: "IX_staff_HomePoolId",
                table: "staff");

            migrationBuilder.DropIndex(
                name: "IX_staff_StaffRoleId",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "AsNumber",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "ContractJobTitle",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "GcmLevel",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "HomePoolId",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "SfiaLevel",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "StaffRoleId",
                table: "staff");
        }
    }
}
