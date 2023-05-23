using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForexFintechAPI.Migrations
{
    /// <inheritdoc />
    public partial class exchangemanipulationmodify : Migration
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

            migrationBuilder.AlterColumn<int>(
                name: "ExchangeRateId",
                table: "Partners",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExchangeManipulationDataId",
                table: "Partners",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "ExchangeManipulationData",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SourceCurrency",
                table: "ExchangeManipulationData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetCurrency",
                table: "ExchangeManipulationData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_ExchangeManipulationData_ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_ExchangeRates_ExchangeRateId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ExchangeManipulationData");

            migrationBuilder.DropColumn(
                name: "SourceCurrency",
                table: "ExchangeManipulationData");

            migrationBuilder.DropColumn(
                name: "TargetCurrency",
                table: "ExchangeManipulationData");

            migrationBuilder.AlterColumn<int>(
                name: "ExchangeRateId",
                table: "Partners",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExchangeManipulationDataId",
                table: "Partners",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_ExchangeManipulationData_ExchangeManipulationDataId",
                table: "Partners",
                column: "ExchangeManipulationDataId",
                principalTable: "ExchangeManipulationData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_ExchangeRates_ExchangeRateId",
                table: "Partners",
                column: "ExchangeRateId",
                principalTable: "ExchangeRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
