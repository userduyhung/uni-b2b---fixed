using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2BMarketplace.Infrastructure.Migrations
{
    public partial class AddPaymentStatusToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add PaymentStatus column to Orders (enum backed by int)
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            // Optional: if you want to seed a default (e.g., Pending -> 0), you can run SQL here.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");
        }
    }
}
