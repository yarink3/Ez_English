using EzEnglish.Api.Domain.Entities;
using EzEnglish.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Infrastructure.Persistence;

/// <summary>
/// Reference data baked into migrations (mascot characters + Phase 4 seed lessons).
/// Lesson content will eventually move to a separate content pipeline / authoring tool.
/// </summary>
internal static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>().HasData(
            new Character
            {
                Id = 1,
                Key = "lion",
                DisplayNameEn = "Leo the Lion",
                DisplayNameHe = "ליאו האריה",
                AvatarUrl = "/img/characters/lion.svg",
            },
            new Character
            {
                Id = 2,
                Key = "bunny",
                DisplayNameEn = "Bella the Bunny",
                DisplayNameHe = "בלה הארנבת",
                AvatarUrl = "/img/characters/bunny.svg",
            },
            new Character
            {
                Id = 3,
                Key = "owl",
                DisplayNameEn = "Ollie the Owl",
                DisplayNameHe = "אולי הינשוף",
                AvatarUrl = "/img/characters/owl.svg",
            },
            new Character
            {
                Id = 4,
                Key = "robot",
                DisplayNameEn = "Rex the Robot",
                DisplayNameHe = "רקס הרובוט",
                AvatarUrl = "/img/characters/robot.svg",
            });

        SeedColorsPreA1(modelBuilder);
        SeedAnimalsPreA1(modelBuilder);
        SeedFamilyPreA1(modelBuilder);
        SeedLettersPreA1(modelBuilder);
        SeedNumbersPreA1(modelBuilder);
    }

    // ---------------- Colors (PreA1) ----------------
    // Lesson 1 (existing): DragMatch — 6 colors across 2 items
    // Lesson 2 (new):      TapPick × 3 — "Find the X"
    // Lesson 3 (new):      Memory — match colors to names
    private static void SeedColorsPreA1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 1, Category = Category.Colors, Level = Level.PreA1, TitleEn = "Colors — first three",  TitleHe = "צבעים — שלושת הראשונים", OrderInLevel = 1 },
            new Lesson { Id = 2, Category = Category.Colors, Level = Level.PreA1, TitleEn = "Colors — listen and find", TitleHe = "צבעים — הקשיבו ובחרו",   OrderInLevel = 2 },
            new Lesson { Id = 3, Category = Category.Colors, Level = Level.PreA1, TitleEn = "Colors — memory match",    TitleHe = "צבעים — משחק זיכרון",   OrderInLevel = 3 });

        modelBuilder.Entity<LessonItem>().HasData(
            // Lesson 1 — DragMatch (existing)
            new LessonItem { Id = 1, LessonId = 1, Kind = LessonItemKind.DragMatch, OrderInLesson = 1, PromptEn = "Match the color to its name", PromptHe = "התאימו את הצבע לשם שלו",
                PayloadJson = "{\"pairs\":[{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"}]}" },
            new LessonItem { Id = 2, LessonId = 1, Kind = LessonItemKind.DragMatch, OrderInLesson = 2, PromptEn = "Match the color to its name", PromptHe = "התאימו את הצבע לשם שלו",
                PayloadJson = "{\"pairs\":[{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"},{\"image\":\"/img/colors/orange.svg\",\"label\":\"orange\"},{\"image\":\"/img/colors/purple.svg\",\"label\":\"purple\"}]}" },

            // Lesson 2 — TapPick × 3
            new LessonItem { Id = 3, LessonId = 2, Kind = LessonItemKind.TapPick, OrderInLesson = 1, PromptEn = "Tap the RED one",    PromptHe = "הקישו על האדום",
                PayloadJson = "{\"correctLabel\":\"red\",\"choices\":[{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"}]}" },
            new LessonItem { Id = 4, LessonId = 2, Kind = LessonItemKind.TapPick, OrderInLesson = 2, PromptEn = "Tap the YELLOW one", PromptHe = "הקישו על הצהוב",
                PayloadJson = "{\"correctLabel\":\"yellow\",\"choices\":[{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"},{\"image\":\"/img/colors/orange.svg\",\"label\":\"orange\"},{\"image\":\"/img/colors/purple.svg\",\"label\":\"purple\"}]}" },
            new LessonItem { Id = 5, LessonId = 2, Kind = LessonItemKind.TapPick, OrderInLesson = 3, PromptEn = "Tap the BLUE one",   PromptHe = "הקישו על הכחול",
                PayloadJson = "{\"correctLabel\":\"blue\",\"choices\":[{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"}]}" },

            // Lesson 3 — Memory
            new LessonItem { Id = 6, LessonId = 3, Kind = LessonItemKind.Memory, OrderInLesson = 1, PromptEn = "Find the matching colors", PromptHe = "מצאו את הזוגות",
                PayloadJson = "{\"pairs\":[{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"},{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"},{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"}]}" });
    }

    // ---------------- Animals (PreA1) ----------------
    private static void SeedAnimalsPreA1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 4, Category = Category.Animals, Level = Level.PreA1, TitleEn = "Farm friends",            TitleHe = "חברים מהחווה",         OrderInLevel = 1 },
            new Lesson { Id = 5, Category = Category.Animals, Level = Level.PreA1, TitleEn = "Animals — listen and find", TitleHe = "חיות — הקשיבו ובחרו", OrderInLevel = 2 },
            new Lesson { Id = 6, Category = Category.Animals, Level = Level.PreA1, TitleEn = "Where do they live?",       TitleHe = "איפה הן גרות?",       OrderInLevel = 3 });

        modelBuilder.Entity<LessonItem>().HasData(
            // Lesson 4 — DragMatch farm + wild
            new LessonItem { Id = 7, LessonId = 4, Kind = LessonItemKind.DragMatch, OrderInLesson = 1, PromptEn = "Match the animal to its name", PromptHe = "התאימו את החיה לשם שלה",
                PayloadJson = "{\"pairs\":[{\"image\":\"🐱\",\"label\":\"cat\"},{\"image\":\"🐶\",\"label\":\"dog\"},{\"image\":\"🐮\",\"label\":\"cow\"}]}" },
            new LessonItem { Id = 8, LessonId = 4, Kind = LessonItemKind.DragMatch, OrderInLesson = 2, PromptEn = "Match the animal to its name", PromptHe = "התאימו את החיה לשם שלה",
                PayloadJson = "{\"pairs\":[{\"image\":\"🐷\",\"label\":\"pig\"},{\"image\":\"🐔\",\"label\":\"hen\"},{\"image\":\"🐑\",\"label\":\"sheep\"}]}" },

            // Lesson 5 — TapPick × 3
            new LessonItem { Id = 9, LessonId = 5, Kind = LessonItemKind.TapPick, OrderInLesson = 1, PromptEn = "Find the CAT", PromptHe = "מצאו את החתול",
                PayloadJson = "{\"correctLabel\":\"cat\",\"choices\":[{\"image\":\"🐱\",\"label\":\"cat\"},{\"image\":\"🐶\",\"label\":\"dog\"},{\"image\":\"🐰\",\"label\":\"rabbit\"}]}" },
            new LessonItem { Id = 10, LessonId = 5, Kind = LessonItemKind.TapPick, OrderInLesson = 2, PromptEn = "Find the LION", PromptHe = "מצאו את האריה",
                PayloadJson = "{\"correctLabel\":\"lion\",\"choices\":[{\"image\":\"🐯\",\"label\":\"tiger\"},{\"image\":\"🦁\",\"label\":\"lion\"},{\"image\":\"🐻\",\"label\":\"bear\"}]}" },
            new LessonItem { Id = 11, LessonId = 5, Kind = LessonItemKind.TapPick, OrderInLesson = 3, PromptEn = "Find the ELEPHANT", PromptHe = "מצאו את הפיל",
                PayloadJson = "{\"correctLabel\":\"elephant\",\"choices\":[{\"image\":\"🦒\",\"label\":\"giraffe\"},{\"image\":\"🐵\",\"label\":\"monkey\"},{\"image\":\"🐘\",\"label\":\"elephant\"}]}" },

            // Lesson 6 — Sort into Farm / Wild bins
            new LessonItem { Id = 12, LessonId = 6, Kind = LessonItemKind.Sort, OrderInLesson = 1, PromptEn = "Drag each animal to its home", PromptHe = "גררו כל חיה לבית שלה",
                PayloadJson =
                    "{\"bins\":[" +
                        "{\"id\":\"farm\",\"label\":\"Farm\",\"emoji\":\"🚜\"}," +
                        "{\"id\":\"wild\",\"label\":\"Wild\",\"emoji\":\"🌳\"}" +
                    "]," +
                    "\"items\":[" +
                        "{\"image\":\"🐮\",\"label\":\"cow\",\"binId\":\"farm\"}," +
                        "{\"image\":\"🐷\",\"label\":\"pig\",\"binId\":\"farm\"}," +
                        "{\"image\":\"🐔\",\"label\":\"hen\",\"binId\":\"farm\"}," +
                        "{\"image\":\"🦁\",\"label\":\"lion\",\"binId\":\"wild\"}," +
                        "{\"image\":\"🐯\",\"label\":\"tiger\",\"binId\":\"wild\"}," +
                        "{\"image\":\"🐘\",\"label\":\"elephant\",\"binId\":\"wild\"}" +
                    "]}" });
    }

    // ---------------- Family (PreA1) ----------------
    private static void SeedFamilyPreA1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 7, Category = Category.Family, Level = Level.PreA1, TitleEn = "My family",                TitleHe = "המשפחה שלי",            OrderInLevel = 1 },
            new Lesson { Id = 8, Category = Category.Family, Level = Level.PreA1, TitleEn = "Family — listen and find", TitleHe = "משפחה — הקשיבו ובחרו",  OrderInLevel = 2 },
            new Lesson { Id = 9, Category = Category.Family, Level = Level.PreA1, TitleEn = "Family memory",            TitleHe = "משפחה — משחק זיכרון",   OrderInLevel = 3 });

        modelBuilder.Entity<LessonItem>().HasData(
            // Lesson 7 — DragMatch
            new LessonItem { Id = 13, LessonId = 7, Kind = LessonItemKind.DragMatch, OrderInLesson = 1, PromptEn = "Match family members to names", PromptHe = "התאימו את בני המשפחה לשמות",
                PayloadJson = "{\"pairs\":[{\"image\":\"👩\",\"label\":\"mom\"},{\"image\":\"👨\",\"label\":\"dad\"},{\"image\":\"👶\",\"label\":\"baby\"}]}" },
            new LessonItem { Id = 14, LessonId = 7, Kind = LessonItemKind.DragMatch, OrderInLesson = 2, PromptEn = "Match family members to names", PromptHe = "התאימו את בני המשפחה לשמות",
                PayloadJson = "{\"pairs\":[{\"image\":\"👦\",\"label\":\"brother\"},{\"image\":\"👧\",\"label\":\"sister\"},{\"image\":\"👵\",\"label\":\"grandma\"}]}" },

            // Lesson 8 — TapPick
            new LessonItem { Id = 15, LessonId = 8, Kind = LessonItemKind.TapPick, OrderInLesson = 1, PromptEn = "Find MOM", PromptHe = "מצאו את אמא",
                PayloadJson = "{\"correctLabel\":\"mom\",\"choices\":[{\"image\":\"👨\",\"label\":\"dad\"},{\"image\":\"👩\",\"label\":\"mom\"},{\"image\":\"👵\",\"label\":\"grandma\"}]}" },
            new LessonItem { Id = 16, LessonId = 8, Kind = LessonItemKind.TapPick, OrderInLesson = 2, PromptEn = "Find BABY", PromptHe = "מצאו את התינוק/ת",
                PayloadJson = "{\"correctLabel\":\"baby\",\"choices\":[{\"image\":\"👦\",\"label\":\"brother\"},{\"image\":\"👧\",\"label\":\"sister\"},{\"image\":\"👶\",\"label\":\"baby\"}]}" },
            new LessonItem { Id = 17, LessonId = 8, Kind = LessonItemKind.TapPick, OrderInLesson = 3, PromptEn = "Find GRANDPA", PromptHe = "מצאו את סבא",
                PayloadJson = "{\"correctLabel\":\"grandpa\",\"choices\":[{\"image\":\"👴\",\"label\":\"grandpa\"},{\"image\":\"👵\",\"label\":\"grandma\"},{\"image\":\"👨\",\"label\":\"dad\"}]}" },

            // Lesson 9 — Memory
            new LessonItem { Id = 18, LessonId = 9, Kind = LessonItemKind.Memory, OrderInLesson = 1, PromptEn = "Find the matching family members", PromptHe = "מצאו את הזוגות",
                PayloadJson = "{\"pairs\":[{\"image\":\"👩\",\"label\":\"mom\"},{\"image\":\"👨\",\"label\":\"dad\"},{\"image\":\"👶\",\"label\":\"baby\"}]}" });
    }

    // ---------------- Letters (PreA1) ----------------
    private static void SeedLettersPreA1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 10, Category = Category.Letters, Level = Level.PreA1, TitleEn = "A, B, C — match it up",    TitleHe = "א, ב, ג — התאימו",        OrderInLevel = 1 },
            new Lesson { Id = 11, Category = Category.Letters, Level = Level.PreA1, TitleEn = "Find the letter",          TitleHe = "מצאו את האות",           OrderInLevel = 2 },
            new Lesson { Id = 12, Category = Category.Letters, Level = Level.PreA1, TitleEn = "Big and small letters",    TitleHe = "אותיות גדולות וקטנות",   OrderInLevel = 3 });

        modelBuilder.Entity<LessonItem>().HasData(
            // Lesson 10 — DragMatch letter ↔ word/thing
            new LessonItem { Id = 19, LessonId = 10, Kind = LessonItemKind.DragMatch, OrderInLesson = 1, PromptEn = "Match each thing to its first letter", PromptHe = "התאימו כל דבר לאות הראשונה שלו",
                PayloadJson = "{\"pairs\":[{\"image\":\"🍎\",\"label\":\"A\"},{\"image\":\"⚽\",\"label\":\"B\"},{\"image\":\"🐱\",\"label\":\"C\"}]}" },
            new LessonItem { Id = 20, LessonId = 10, Kind = LessonItemKind.DragMatch, OrderInLesson = 2, PromptEn = "Match each thing to its first letter", PromptHe = "התאימו כל דבר לאות הראשונה שלו",
                PayloadJson = "{\"pairs\":[{\"image\":\"🐶\",\"label\":\"D\"},{\"image\":\"🥚\",\"label\":\"E\"},{\"image\":\"🐟\",\"label\":\"F\"}]}" },

            // Lesson 11 — TapPick — find the letter
            new LessonItem { Id = 21, LessonId = 11, Kind = LessonItemKind.TapPick, OrderInLesson = 1, PromptEn = "Tap the letter A", PromptHe = "הקישו על האות A",
                PayloadJson = "{\"correctLabel\":\"A\",\"choices\":[{\"image\":\"A\",\"label\":\"A\"},{\"image\":\"E\",\"label\":\"E\"},{\"image\":\"O\",\"label\":\"O\"}]}" },
            new LessonItem { Id = 22, LessonId = 11, Kind = LessonItemKind.TapPick, OrderInLesson = 2, PromptEn = "Tap the letter B", PromptHe = "הקישו על האות B",
                PayloadJson = "{\"correctLabel\":\"B\",\"choices\":[{\"image\":\"D\",\"label\":\"D\"},{\"image\":\"B\",\"label\":\"B\"},{\"image\":\"P\",\"label\":\"P\"}]}" },
            new LessonItem { Id = 23, LessonId = 11, Kind = LessonItemKind.TapPick, OrderInLesson = 3, PromptEn = "Tap the letter S", PromptHe = "הקישו על האות S",
                PayloadJson = "{\"correctLabel\":\"S\",\"choices\":[{\"image\":\"S\",\"label\":\"S\"},{\"image\":\"Z\",\"label\":\"Z\"},{\"image\":\"X\",\"label\":\"X\"}]}" },

            // Lesson 12 — Memory uppercase ↔ lowercase
            new LessonItem { Id = 24, LessonId = 12, Kind = LessonItemKind.Memory, OrderInLesson = 1, PromptEn = "Match each big letter with its small letter", PromptHe = "התאימו כל אות גדולה לאות הקטנה",
                PayloadJson = "{\"pairs\":[{\"image\":\"A\",\"label\":\"a\"},{\"image\":\"B\",\"label\":\"b\"},{\"image\":\"C\",\"label\":\"c\"}]}" });
    }

    // ---------------- Numbers (PreA1) ----------------
    private static void SeedNumbersPreA1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 13, Category = Category.Numbers, Level = Level.PreA1, TitleEn = "1, 2, 3 — count them", TitleHe = "1, 2, 3 — סופרים",      OrderInLevel = 1 },
            new Lesson { Id = 14, Category = Category.Numbers, Level = Level.PreA1, TitleEn = "Count and pick",       TitleHe = "סופרים ובוחרים",       OrderInLevel = 2 },
            new Lesson { Id = 15, Category = Category.Numbers, Level = Level.PreA1, TitleEn = "Number memory",        TitleHe = "מספרים — משחק זיכרון", OrderInLevel = 3 });

        modelBuilder.Entity<LessonItem>().HasData(
            // Lesson 13 — DragMatch count ↔ digit
            new LessonItem { Id = 25, LessonId = 13, Kind = LessonItemKind.DragMatch, OrderInLesson = 1, PromptEn = "Match the picture to the number", PromptHe = "התאימו את התמונה למספר",
                PayloadJson = "{\"pairs\":[{\"image\":\"🍎\",\"label\":\"1\"},{\"image\":\"🍎🍎\",\"label\":\"2\"},{\"image\":\"🍎🍎🍎\",\"label\":\"3\"}]}" },
            new LessonItem { Id = 26, LessonId = 13, Kind = LessonItemKind.DragMatch, OrderInLesson = 2, PromptEn = "Match the picture to the number", PromptHe = "התאימו את התמונה למספר",
                PayloadJson = "{\"pairs\":[{\"image\":\"⭐⭐⭐⭐\",\"label\":\"4\"},{\"image\":\"⭐⭐⭐⭐⭐\",\"label\":\"5\"},{\"image\":\"⭐⭐⭐⭐⭐⭐\",\"label\":\"6\"}]}" },

            // Lesson 14 — TapPick count
            new LessonItem { Id = 27, LessonId = 14, Kind = LessonItemKind.TapPick, OrderInLesson = 1, PromptEn = "How many apples? Tap the number.", PromptHe = "כמה תפוחים? הקישו על המספר.",
                PayloadJson = "{\"correctLabel\":\"3\",\"choices\":[{\"image\":\"1\",\"label\":\"1\"},{\"image\":\"3\",\"label\":\"3\"},{\"image\":\"5\",\"label\":\"5\"}]}" },
            new LessonItem { Id = 28, LessonId = 14, Kind = LessonItemKind.TapPick, OrderInLesson = 2, PromptEn = "Tap the number TWO", PromptHe = "הקישו על המספר שתיים",
                PayloadJson = "{\"correctLabel\":\"2\",\"choices\":[{\"image\":\"4\",\"label\":\"4\"},{\"image\":\"2\",\"label\":\"2\"},{\"image\":\"7\",\"label\":\"7\"}]}" },
            new LessonItem { Id = 29, LessonId = 14, Kind = LessonItemKind.TapPick, OrderInLesson = 3, PromptEn = "Tap the number FIVE", PromptHe = "הקישו על המספר חמש",
                PayloadJson = "{\"correctLabel\":\"5\",\"choices\":[{\"image\":\"3\",\"label\":\"3\"},{\"image\":\"8\",\"label\":\"8\"},{\"image\":\"5\",\"label\":\"5\"}]}" },

            // Lesson 15 — Memory number ↔ word
            new LessonItem { Id = 30, LessonId = 15, Kind = LessonItemKind.Memory, OrderInLesson = 1, PromptEn = "Match each number to its name", PromptHe = "התאימו כל מספר לשם שלו",
                PayloadJson = "{\"pairs\":[{\"image\":\"1\",\"label\":\"one\"},{\"image\":\"2\",\"label\":\"two\"},{\"image\":\"3\",\"label\":\"three\"}]}" });
    }
}

