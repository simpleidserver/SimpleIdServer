using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherLifetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DpopLifetimeSeconds",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxBindingMessageSize",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxRequestParameterLifetimeSeconds",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DpopLifetimeSeconds",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "MaxBindingMessageSize",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "MaxRequestParameterLifetimeSeconds",
                table: "Clients");
        }
    }
}
