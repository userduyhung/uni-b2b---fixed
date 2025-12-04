using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2BMarketplace.Infrastructure.Migrations
{
    public partial class AddOrderIdToPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add OrderId column (string) to Payments table so payments can be linked to orders
            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Payments",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            // Optional: create index on OrderId for faster lookups
            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Payments");
        }
    }
}
