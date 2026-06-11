using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EzEnglish.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedExpandedLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Category", "Level", "OrderInLevel", "TitleEn", "TitleHe" },
                values: new object[,]
                {
                    { 2, 1, 1, 2, "Colors — listen and find", "צבעים — הקשיבו ובחרו" },
                    { 3, 1, 1, 3, "Colors — memory match", "צבעים — משחק זיכרון" },
                    { 4, 2, 1, 1, "Farm friends", "חברים מהחווה" },
                    { 5, 2, 1, 2, "Animals — listen and find", "חיות — הקשיבו ובחרו" },
                    { 6, 2, 1, 3, "Where do they live?", "איפה הן גרות?" },
                    { 7, 3, 1, 1, "My family", "המשפחה שלי" },
                    { 8, 3, 1, 2, "Family — listen and find", "משפחה — הקשיבו ובחרו" },
                    { 9, 3, 1, 3, "Family memory", "משפחה — משחק זיכרון" },
                    { 10, 4, 1, 1, "A, B, C — match it up", "א, ב, ג — התאימו" },
                    { 11, 4, 1, 2, "Find the letter", "מצאו את האות" },
                    { 12, 4, 1, 3, "Big and small letters", "אותיות גדולות וקטנות" },
                    { 13, 5, 1, 1, "1, 2, 3 — count them", "1, 2, 3 — סופרים" },
                    { 14, 5, 1, 2, "Count and pick", "סופרים ובוחרים" },
                    { 15, 5, 1, 3, "Number memory", "מספרים — משחק זיכרון" }
                });

            migrationBuilder.InsertData(
                table: "LessonItems",
                columns: new[] { "Id", "Kind", "LessonId", "OrderInLesson", "PayloadJson", "PromptEn", "PromptHe" },
                values: new object[,]
                {
                    { 3, 2, 2, 1, "{\"correctLabel\":\"red\",\"choices\":[{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"}]}", "Tap the RED one", "הקישו על האדום" },
                    { 4, 2, 2, 2, "{\"correctLabel\":\"yellow\",\"choices\":[{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"},{\"image\":\"/img/colors/orange.svg\",\"label\":\"orange\"},{\"image\":\"/img/colors/purple.svg\",\"label\":\"purple\"}]}", "Tap the YELLOW one", "הקישו על הצהוב" },
                    { 5, 2, 2, 3, "{\"correctLabel\":\"blue\",\"choices\":[{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"}]}", "Tap the BLUE one", "הקישו על הכחול" },
                    { 6, 4, 3, 1, "{\"pairs\":[{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"}]}", "Find the matching colors", "מצאו את הזוגות" },
                    { 7, 1, 4, 1, "{\"pairs\":[{\"image\":\"🐱\",\"label\":\"cat\"},{\"image\":\"🐶\",\"label\":\"dog\"},{\"image\":\"🐮\",\"label\":\"cow\"}]}", "Match the animal to its name", "התאימו את החיה לשם שלה" },
                    { 8, 1, 4, 2, "{\"pairs\":[{\"image\":\"🐷\",\"label\":\"pig\"},{\"image\":\"🐔\",\"label\":\"hen\"},{\"image\":\"🐑\",\"label\":\"sheep\"}]}", "Match the animal to its name", "התאימו את החיה לשם שלה" },
                    { 9, 2, 5, 1, "{\"correctLabel\":\"cat\",\"choices\":[{\"image\":\"🐱\",\"label\":\"cat\"},{\"image\":\"🐶\",\"label\":\"dog\"},{\"image\":\"🐰\",\"label\":\"rabbit\"}]}", "Find the CAT", "מצאו את החתול" },
                    { 10, 2, 5, 2, "{\"correctLabel\":\"lion\",\"choices\":[{\"image\":\"🐯\",\"label\":\"tiger\"},{\"image\":\"🦁\",\"label\":\"lion\"},{\"image\":\"🐻\",\"label\":\"bear\"}]}", "Find the LION", "מצאו את האריה" },
                    { 11, 2, 5, 3, "{\"correctLabel\":\"elephant\",\"choices\":[{\"image\":\"🦒\",\"label\":\"giraffe\"},{\"image\":\"🐵\",\"label\":\"monkey\"},{\"image\":\"🐘\",\"label\":\"elephant\"}]}", "Find the ELEPHANT", "מצאו את הפיל" },
                    { 12, 5, 6, 1, "{\"bins\":[{\"id\":\"farm\",\"label\":\"Farm\",\"emoji\":\"🚜\"},{\"id\":\"wild\",\"label\":\"Wild\",\"emoji\":\"🌳\"}],\"items\":[{\"image\":\"🐮\",\"label\":\"cow\",\"binId\":\"farm\"},{\"image\":\"🐷\",\"label\":\"pig\",\"binId\":\"farm\"},{\"image\":\"🐔\",\"label\":\"hen\",\"binId\":\"farm\"},{\"image\":\"🦁\",\"label\":\"lion\",\"binId\":\"wild\"},{\"image\":\"🐯\",\"label\":\"tiger\",\"binId\":\"wild\"},{\"image\":\"🐘\",\"label\":\"elephant\",\"binId\":\"wild\"}]}", "Drag each animal to its home", "גררו כל חיה לבית שלה" },
                    { 13, 1, 7, 1, "{\"pairs\":[{\"image\":\"👩\",\"label\":\"mom\"},{\"image\":\"👨\",\"label\":\"dad\"},{\"image\":\"👶\",\"label\":\"baby\"}]}", "Match family members to names", "התאימו את בני המשפחה לשמות" },
                    { 14, 1, 7, 2, "{\"pairs\":[{\"image\":\"👦\",\"label\":\"brother\"},{\"image\":\"👧\",\"label\":\"sister\"},{\"image\":\"👵\",\"label\":\"grandma\"}]}", "Match family members to names", "התאימו את בני המשפחה לשמות" },
                    { 15, 2, 8, 1, "{\"correctLabel\":\"mom\",\"choices\":[{\"image\":\"👨\",\"label\":\"dad\"},{\"image\":\"👩\",\"label\":\"mom\"},{\"image\":\"👵\",\"label\":\"grandma\"}]}", "Find MOM", "מצאו את אמא" },
                    { 16, 2, 8, 2, "{\"correctLabel\":\"baby\",\"choices\":[{\"image\":\"👦\",\"label\":\"brother\"},{\"image\":\"👧\",\"label\":\"sister\"},{\"image\":\"👶\",\"label\":\"baby\"}]}", "Find BABY", "מצאו את התינוק/ת" },
                    { 17, 2, 8, 3, "{\"correctLabel\":\"grandpa\",\"choices\":[{\"image\":\"👴\",\"label\":\"grandpa\"},{\"image\":\"👵\",\"label\":\"grandma\"},{\"image\":\"👨\",\"label\":\"dad\"}]}", "Find GRANDPA", "מצאו את סבא" },
                    { 18, 4, 9, 1, "{\"pairs\":[{\"image\":\"👩\",\"label\":\"mom\"},{\"image\":\"👨\",\"label\":\"dad\"},{\"image\":\"👶\",\"label\":\"baby\"}]}", "Find the matching family members", "מצאו את הזוגות" },
                    { 19, 1, 10, 1, "{\"pairs\":[{\"image\":\"🍎\",\"label\":\"A\"},{\"image\":\"⚽\",\"label\":\"B\"},{\"image\":\"🐱\",\"label\":\"C\"}]}", "Match each thing to its first letter", "התאימו כל דבר לאות הראשונה שלו" },
                    { 20, 1, 10, 2, "{\"pairs\":[{\"image\":\"🐶\",\"label\":\"D\"},{\"image\":\"🥚\",\"label\":\"E\"},{\"image\":\"🐟\",\"label\":\"F\"}]}", "Match each thing to its first letter", "התאימו כל דבר לאות הראשונה שלו" },
                    { 21, 2, 11, 1, "{\"correctLabel\":\"A\",\"choices\":[{\"image\":\"A\",\"label\":\"A\"},{\"image\":\"E\",\"label\":\"E\"},{\"image\":\"O\",\"label\":\"O\"}]}", "Tap the letter A", "הקישו על האות A" },
                    { 22, 2, 11, 2, "{\"correctLabel\":\"B\",\"choices\":[{\"image\":\"D\",\"label\":\"D\"},{\"image\":\"B\",\"label\":\"B\"},{\"image\":\"P\",\"label\":\"P\"}]}", "Tap the letter B", "הקישו על האות B" },
                    { 23, 2, 11, 3, "{\"correctLabel\":\"S\",\"choices\":[{\"image\":\"S\",\"label\":\"S\"},{\"image\":\"Z\",\"label\":\"Z\"},{\"image\":\"X\",\"label\":\"X\"}]}", "Tap the letter S", "הקישו על האות S" },
                    { 24, 4, 12, 1, "{\"pairs\":[{\"image\":\"A\",\"label\":\"a\"},{\"image\":\"B\",\"label\":\"b\"},{\"image\":\"C\",\"label\":\"c\"}]}", "Match each big letter with its small letter", "התאימו כל אות גדולה לאות הקטנה" },
                    { 25, 1, 13, 1, "{\"pairs\":[{\"image\":\"🍎\",\"label\":\"1\"},{\"image\":\"🍎🍎\",\"label\":\"2\"},{\"image\":\"🍎🍎🍎\",\"label\":\"3\"}]}", "Match the picture to the number", "התאימו את התמונה למספר" },
                    { 26, 1, 13, 2, "{\"pairs\":[{\"image\":\"⭐⭐⭐⭐\",\"label\":\"4\"},{\"image\":\"⭐⭐⭐⭐⭐\",\"label\":\"5\"},{\"image\":\"⭐⭐⭐⭐⭐⭐\",\"label\":\"6\"}]}", "Match the picture to the number", "התאימו את התמונה למספר" },
                    { 27, 2, 14, 1, "{\"correctLabel\":\"3\",\"choices\":[{\"image\":\"1\",\"label\":\"1\"},{\"image\":\"3\",\"label\":\"3\"},{\"image\":\"5\",\"label\":\"5\"}]}", "How many apples? Tap the number.", "כמה תפוחים? הקישו על המספר." },
                    { 28, 2, 14, 2, "{\"correctLabel\":\"2\",\"choices\":[{\"image\":\"4\",\"label\":\"4\"},{\"image\":\"2\",\"label\":\"2\"},{\"image\":\"7\",\"label\":\"7\"}]}", "Tap the number TWO", "הקישו על המספר שתיים" },
                    { 29, 2, 14, 3, "{\"correctLabel\":\"5\",\"choices\":[{\"image\":\"3\",\"label\":\"3\"},{\"image\":\"8\",\"label\":\"8\"},{\"image\":\"5\",\"label\":\"5\"}]}", "Tap the number FIVE", "הקישו על המספר חמש" },
                    { 30, 4, 15, 1, "{\"pairs\":[{\"image\":\"1\",\"label\":\"one\"},{\"image\":\"2\",\"label\":\"two\"},{\"image\":\"3\",\"label\":\"three\"}]}", "Match each number to its name", "התאימו כל מספר לשם שלו" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "LessonItems",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
