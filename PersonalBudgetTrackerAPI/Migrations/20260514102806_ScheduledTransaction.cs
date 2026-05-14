using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalBudgetTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class ScheduledTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsIncome = table.Column<bool>(type: "bit", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    PaymentGatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionPartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsFlexibleAmount = table.Column<bool>(type: "bit", nullable: false),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    DayOfMonth = table.Column<int>(type: "int", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ScheduledPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledPayments_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledPayments_PaymentGateway_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateway",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledPayments_Reason_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "Reason",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScheduledPayments_TransactionPartner_TransactionPartnerId",
                        column: x => x.TransactionPartnerId,
                        principalTable: "TransactionPartner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTransactionOccurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTransactionOccurrences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTransactionOccurrences_ScheduledPayments_ScheduledTransactionId",
                        column: x => x.ScheduledTransactionId,
                        principalTable: "ScheduledPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPayments_CategoryId",
                table: "ScheduledPayments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPayments_PaymentGatewayId",
                table: "ScheduledPayments",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPayments_ReasonId",
                table: "ScheduledPayments",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledPayments_TransactionPartnerId",
                table: "ScheduledPayments",
                column: "TransactionPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTransactionOccurrences_ScheduledTransactionId",
                table: "ScheduledTransactionOccurrences",
                column: "ScheduledTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledTransactionOccurrences");

            migrationBuilder.DropTable(
                name: "ScheduledPayments");
        }
    }
}
