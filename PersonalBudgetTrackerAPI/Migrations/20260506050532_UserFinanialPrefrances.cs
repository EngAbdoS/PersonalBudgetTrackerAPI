using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalBudgetTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserFinanialPrefrances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScopeType = table.Column<int>(type: "int", nullable: false),
                    PaymentGatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecurrenceMode = table.Column<int>(type: "int", nullable: false),
                    RecurrencePeriod = table.Column<int>(type: "int", nullable: true),
                    LastActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionPartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialRules_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FinancialRules_PaymentGateway_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateway",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FinancialRules_TransactionPartner_TransactionPartnerId",
                        column: x => x.TransactionPartnerId,
                        principalTable: "TransactionPartner",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialRules_CategoryId",
                table: "FinancialRules",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialRules_PaymentGatewayId",
                table: "FinancialRules",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialRules_TransactionPartnerId",
                table: "FinancialRules",
                column: "TransactionPartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialRules");
        }
    }
}
