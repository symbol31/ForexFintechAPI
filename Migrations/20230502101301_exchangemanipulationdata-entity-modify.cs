using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForexFintechAPI.Migrations
{
    /// <inheritdoc />
    public partial class exchangemanipulationdataentitymodify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ExchangeManipulationDataId",
                table: "Partners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PartnerCode",
                table: "ExchangeManipulationData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ExchangeManipulationDataId",
                table: "Partners",
                column: "ExchangeManipulationDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_ExchangeManipulationData_ExchangeManipulationDataId",
                table: "Partners",
                column: "ExchangeManipulationDataId",
                principalTable: "ExchangeManipulationData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_ExchangeManipulationData_ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ExchangeManipulationDataId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PartnerCode",
                table: "ExchangeManipulationData");

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "ExchangeManipulationData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeManipulationData_PartnerId",
                table: "ExchangeManipulationData",
                column: "PartnerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
