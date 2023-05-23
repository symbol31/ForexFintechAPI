using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForexFintechAPI.Migrations
{
    /// <inheritdoc />
    public partial class modify_exchangerate_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "To",
                table: "ExchangeRates",
                newName: "Target_Currency");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "ExchangeRates",
                newName: "Exchange_Rate");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "ExchangeRates",
                newName: "Source_Currency");

            migrationBuilder.AddColumn<string>(
                name: "Api_Key",
                table: "ExchangeRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Cache_Expiry",
                table: "ExchangeRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Service_Fee",
                table: "ExchangeRates",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "ExchangeRates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Api_Key",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Cache_Expiry",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Service_Fee",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ExchangeRates");

            migrationBuilder.RenameColumn(
                name: "Target_Currency",
                table: "ExchangeRates",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "Source_Currency",
                table: "ExchangeRates",
                newName: "From");

            migrationBuilder.RenameColumn(
                name: "Exchange_Rate",
                table: "ExchangeRates",
                newName: "Rate");
        }
    }
}
