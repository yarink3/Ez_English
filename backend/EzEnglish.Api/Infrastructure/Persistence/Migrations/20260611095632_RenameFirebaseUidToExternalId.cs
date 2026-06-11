using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzEnglish.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameFirebaseUidToExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirebaseUid",
                table: "Parents",
                newName: "ExternalId");

            migrationBuilder.RenameIndex(
                name: "IX_Parents_FirebaseUid",
                table: "Parents",
                newName: "IX_Parents_ExternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "Parents",
                newName: "FirebaseUid");

            migrationBuilder.RenameIndex(
                name: "IX_Parents_ExternalId",
                table: "Parents",
                newName: "IX_Parents_FirebaseUid");
        }
    }
}
