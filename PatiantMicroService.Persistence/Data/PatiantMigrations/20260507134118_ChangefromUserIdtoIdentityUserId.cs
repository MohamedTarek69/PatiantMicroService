using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatiantMicroService.Persistence.Data.PatiantMigrations
{
    /// <inheritdoc />
    public partial class ChangefromUserIdtoIdentityUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Patients",
                newName: "IdentityUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "Patients",
                newName: "UserId");
        }
    }
}
