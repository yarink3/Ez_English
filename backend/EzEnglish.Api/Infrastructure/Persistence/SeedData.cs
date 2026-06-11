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
    }

    private static void SeedColorsPreA1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>().HasData(new Lesson
        {
            Id = 1,
            Category = Category.Colors,
            Level = Level.PreA1,
            TitleEn = "Colors — first three",
            TitleHe = "צבעים — שלושת הראשונים",
            OrderInLevel = 1,
        });

        modelBuilder.Entity<LessonItem>().HasData(
            new LessonItem
            {
                Id = 1,
                LessonId = 1,
                Kind = LessonItemKind.DragMatch,
                OrderInLesson = 1,
                PromptEn = "Match the color to its name",
                PromptHe = "התאימו את הצבע לשם שלו",
                PayloadJson =
                    "{\"pairs\":[" +
                    "{\"image\":\"/img/colors/red.svg\",\"label\":\"red\"}," +
                    "{\"image\":\"/img/colors/blue.svg\",\"label\":\"blue\"}," +
                    "{\"image\":\"/img/colors/yellow.svg\",\"label\":\"yellow\"}" +
                    "]}",
            },
            new LessonItem
            {
                Id = 2,
                LessonId = 1,
                Kind = LessonItemKind.DragMatch,
                OrderInLesson = 2,
                PromptEn = "Match the color to its name",
                PromptHe = "התאימו את הצבע לשם שלו",
                PayloadJson =
                    "{\"pairs\":[" +
                    "{\"image\":\"/img/colors/green.svg\",\"label\":\"green\"}," +
                    "{\"image\":\"/img/colors/orange.svg\",\"label\":\"orange\"}," +
                    "{\"image\":\"/img/colors/purple.svg\",\"label\":\"purple\"}" +
                    "]}",
            });
    }
}

