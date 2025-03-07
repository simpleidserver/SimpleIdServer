using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqliteMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringJobStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurringJobStatusLst",
                columns: table => new
                {
                    JobId = table.Column<string>(type: "TEXT", nullable: false),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringJobStatusLst", x => x.JobId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringJobStatusLst");
        }
    }
}
