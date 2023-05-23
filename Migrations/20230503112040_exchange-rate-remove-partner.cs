using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForexFintechAPI.Migrations
{
    /// <inheritdoc />
    public partial class exchangerateremovepartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_ExchangeManipulationData_ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_ExchangeRates_ExchangeRateId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ExchangeRateId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ExchangeRateId",
                table: "Partners");

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "ExchangeManipulationData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "ExchangeManipulationData",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeManipulationData_PartnerId",
                table: "ExchangeManipulationData",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeManipulationData_PartnerId",
                table: "ExchangeManipulationData");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "ExchangeManipulationData");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ExchangeManipulationData");

            migrationBuilder.AddColumn<int>(
                name: "ExchangeManipulationDataId",
                table: "Partners",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExchangeRateId",
                table: "Partners",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ExchangeManipulationDataId",
                table: "Partners",
                column: "ExchangeManipulationDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ExchangeRateId",
                table: "Partners",
                column: "ExchangeRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_ExchangeManipulationData_ExchangeManipulationDataId",
                table: "Partners",
                column: "ExchangeManipulationDataId",
                principalTable: "ExchangeManipulationData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_ExchangeRates_ExchangeRateId",
                table: "Partners",
                column: "ExchangeRateId",
                principalTable: "ExchangeRates",
                principalColumn: "Id");
        }
    }
}
