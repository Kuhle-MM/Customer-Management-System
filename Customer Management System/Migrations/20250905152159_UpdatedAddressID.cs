using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAddressID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Address_AddressId",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Customers",
                newName: "AddressID");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_AddressId",
                table: "Customers",
                newName: "IX_Customers_AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Address_AddressID",
                table: "Customers",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Address_AddressID",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "AddressID",
                table: "Customers",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_AddressID",
                table: "Customers",
                newName: "IX_Customers_AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Address_AddressId",
                table: "Customers",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
