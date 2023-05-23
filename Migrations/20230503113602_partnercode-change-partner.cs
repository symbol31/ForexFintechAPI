using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForexFintechAPI.Migrations
{
    /// <inheritdoc />
    public partial class partnercodechangepartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData");

            migrationBuilder.DropColumn(
                name: "PartnerCode",
                table: "ExchangeManipulationData");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "ExchangeManipulationData",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "ExchangeManipulationData",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PartnerCode",
                table: "ExchangeManipulationData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeManipulationData_Partners_PartnerId",
                table: "ExchangeManipulationData",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id");
        }
    }
}
