using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class UpdateRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationRecord");

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId",
                principalTable: "ProvisioningConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationId",
                principalTable: "ProvisioningConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationRecord");

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId",
                principalTable: "ProvisioningConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationId",
                principalTable: "ProvisioningConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
