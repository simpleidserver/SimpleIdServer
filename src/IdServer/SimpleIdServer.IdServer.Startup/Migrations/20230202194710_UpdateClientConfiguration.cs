using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.Startup.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Clients_ClientId",
                table: "Translations");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Clients_ClientId",
                table: "Translations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Clients_ClientId",
                table: "Translations");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Clients_ClientId",
                table: "Translations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId");
        }
    }
}
