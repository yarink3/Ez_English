using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EzEnglish.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedColorsLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Category", "Level", "OrderInLevel", "TitleEn", "TitleHe" },
                values: new object[] { 1, 1, 1, 1, "Colors — first three", "צבעים — שלושת הראשונים" });

            migrationBuilder.InsertData(
                table: "LessonItems",
                columns: new[] { "Id", "Kind", "LessonId", "OrderInLesson", "PayloadJson", "PromptEn", "PromptHe" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "{\"pairs\":[{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"}]}", "Match the color to its name", "התאימו את הצבע לשם שלו" },
                    { 2, 1, 1, 2, "{\"pairs\":[{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"},{\"image\":\"/img/colors/orange.svg\",\"label\":\"orange\"},{\"image\":\"/img/colors/purple.svg\",\"label\":\"purple\"}]}", "Match the color to its name", "התאימו את הצבע לשם שלו" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
