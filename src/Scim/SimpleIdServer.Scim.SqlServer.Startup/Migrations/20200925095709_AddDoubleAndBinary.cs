using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class AddDoubleAndBinary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ValueByte",
                table: "SCIMRepresentationAttributeValueLst",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDecimal",
                table: "SCIMRepresentationAttributeValueLst",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueByte",
                table: "SCIMRepresentationAttributeValueLst");

            migrationBuilder.DropColumn(
                name: "ValueDecimal",
                table: "SCIMRepresentationAttributeValueLst");
        }
    }
}
