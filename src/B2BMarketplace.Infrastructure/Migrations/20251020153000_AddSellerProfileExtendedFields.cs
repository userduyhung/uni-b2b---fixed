using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2BMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSellerProfileExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add HasVerifiedBadge column to SellerProfiles table
            migrationBuilder.AddColumn<bool>(
                name: "HasVerifiedBadge",
                table: "SellerProfiles",
                type: "INTEGER", // Using INTEGER for boolean in PostgreSQL
                nullable: false,
                defaultValue: false);

            // Add BusinessName column to SellerProfiles table
            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "SellerProfiles",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            // Add PrimaryCategoryId column to SellerProfiles table
            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryCategoryId",
                table: "SellerProfiles",
                type: "TEXT",
                nullable: true);

            // Add index for PrimaryCategoryId
            migrationBuilder.CreateIndex(
                name: "IX_SellerProfiles_PrimaryCategoryId",
                table: "SellerProfiles",
                column: "PrimaryCategoryId");

            // Add foreign key constraint for PrimaryCategoryId
            migrationBuilder.AddForeignKey(
                name: "FK_SellerProfiles_ProductCategories_PrimaryCategoryId",
                table: "SellerProfiles",
                column: "PrimaryCategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Add new columns to CategoryConfiguration table
            migrationBuilder.AddColumn<bool>(
                name: "AllowsVerifiedBadge",
                table: "CategoryConfiguration",
                type: "INTEGER", // Using INTEGER for boolean in PostgreSQL
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MinCertificationsForBadge",
                table: "CategoryConfiguration",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraint first
            migrationBuilder.DropForeignKey(
                name: "FK_SellerProfiles_ProductCategories_PrimaryCategoryId",
                table: "SellerProfiles");

            // Remove index
            migrationBuilder.DropIndex(
                name: "IX_SellerProfiles_PrimaryCategoryId",
                table: "SellerProfiles");

            // Remove columns from CategoryConfiguration table
            migrationBuilder.DropColumn(
                name: "AllowsVerifiedBadge",
                table: "CategoryConfiguration");

            migrationBuilder.DropColumn(
                name: "MinCertificationsForBadge",
                table: "CategoryConfiguration");

            // Remove columns from SellerProfiles table
            migrationBuilder.DropColumn(
                name: "HasVerifiedBadge",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "SellerProfiles");

            migrationBuilder.DropColumn(
                name: "PrimaryCategoryId",
                table: "SellerProfiles");
        }
    }
}