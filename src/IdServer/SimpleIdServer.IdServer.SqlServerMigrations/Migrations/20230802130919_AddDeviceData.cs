using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserDevice");

            migrationBuilder.RenameColumn(
                name: "SerializedOptions",
                table: "UserDevice",
                newName: "Version");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDateTime",
                table: "UserDevice",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PushToken",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PushType",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDateTime",
                table: "UserDevice");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "UserDevice");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "UserDevice");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "UserDevice");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserDevice");

            migrationBuilder.DropColumn(
                name: "PushToken",
                table: "UserDevice");

            migrationBuilder.DropColumn(
                name: "PushType",
                table: "UserDevice");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "UserDevice",
                newName: "SerializedOptions");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserDevice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
