using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddDistributedCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationDefinitionRecordValueId",
                table: "Translations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationDefinitionRecordValueId1",
                table: "Translations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullQualifiedName",
                table: "Definitions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationKeyPairValueRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationDefinitionRecordValue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ConfigurationDefinitionRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValues",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DistributedCache",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(449)", maxLength: 449, nullable: false),
                    Value = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ExpiresAtTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SlidingExpirationInSeconds = table.Column<long>(type: "bigint", nullable: true),
                    AbsoluteExpiration = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributedCache", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId1",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValues_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation",
                column: "ConfigurationDefinitionRecordValueId",
                principalTable: "ConfigurationDefinitionRecordValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId",
                principalTable: "ConfigurationDefinitionRecordValue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId1",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId1",
                principalTable: "ConfigurationDefinitionRecordValue",
                principalColumn: "Id");
            migrationBuilder.Sql("ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValues_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId1",
                table: "Translations");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordValues");

            migrationBuilder.DropTable(
                name: "DistributedCache");

            migrationBuilder.DropIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId1",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "ConfigurationDefinitionRecordValueId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "ConfigurationDefinitionRecordValueId1",
                table: "Translations");

            migrationBuilder.AlterColumn<string>(
                name: "FullQualifiedName",
                table: "Definitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationKeyPairValueRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationDefinitionRecordValue",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ConfigurationDefinitionRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation",
                column: "ConfigurationDefinitionRecordValueId",
                principalTable: "ConfigurationDefinitionRecordValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
