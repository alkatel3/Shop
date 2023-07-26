using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RenamedFieldInOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentData",
                table: "OrderHeaders",
                newName: "PaymentDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "OrderHeaders",
                newName: "PaymentData");
        }
    }
}
