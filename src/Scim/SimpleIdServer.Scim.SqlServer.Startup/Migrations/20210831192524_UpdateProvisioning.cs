using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class UpdateProvisioning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory");

            migrationBuilder.DropIndex(
                name: "IX_ProvisioningConfigurationHistory_ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory");

            migrationBuilder.DropColumn(
                name: "ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationHistory_ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId1",
                principalTable: "ProvisioningConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
