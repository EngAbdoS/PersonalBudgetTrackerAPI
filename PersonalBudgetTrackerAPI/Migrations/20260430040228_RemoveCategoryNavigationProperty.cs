using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalBudgetTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPartner_Category_CategoryId",
                table: "TransactionPartner");

            migrationBuilder.DropIndex(
                name: "IX_TransactionPartner_CategoryId",
                table: "TransactionPartner");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TransactionPartner");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "TransactionPartner",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionPartner_CategoryId",
                table: "TransactionPartner",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPartner_Category_CategoryId",
                table: "TransactionPartner",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
