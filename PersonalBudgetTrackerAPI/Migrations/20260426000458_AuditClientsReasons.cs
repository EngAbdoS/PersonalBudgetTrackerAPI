using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalBudgetTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AuditClientsReasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Reason",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Reason",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Reason",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Reason",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reason",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reason",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Reason",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Reason");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Reason");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Reason");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Reason");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reason");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reason");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Reason");
        }
    }
}
