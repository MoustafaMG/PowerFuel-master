using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerFuel.Infrastructure.Migrations;

public partial class AddPayments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StripePaymentIntentId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Amount = table.Column<long>(type: "bigint", nullable: false),
                Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CustomerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Payments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Payments_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Payments_OrderId",
            table: "Payments",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_Payments_UserId",
            table: "Payments",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Payments_StripePaymentIntentId",
            table: "Payments",
            column: "StripePaymentIntentId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Payments");
    }
}

