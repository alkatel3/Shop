using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RenamedPropInOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShoppingDate",
                table: "OrderHeaders",
                newName: "ShippingDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingDate",
                table: "OrderHeaders",
                newName: "ShoppingDate");
        }
    }
}
