using EzEnglish.Api.Domain.Enums;

namespace EzEnglish.Api.Domain.Entities;

public sealed class Parent
{
    public int Id { get; set; }
    /// <summary>
    /// Firebase Authentication UID for this parent. Populated from the
    /// <c>user_id</c> / <c>sub</c> claim of the verified Firebase ID token.
    /// </summary>
    public required string FirebaseUid { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Child> Children { get; set; } = new List<Child>();
}

public sealed class Character
{
    public int Id { get; set; }
    public required string Key { get; set; }              // stable slug, e.g. "lion"
    public required string DisplayNameEn { get; set; }
    public required string DisplayNameHe { get; set; }
    public required string AvatarUrl { get; set; }
}

public sealed class Child
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public Parent Parent { get; set; } = null!;

    public required string DisplayName { get; set; }
    public DateOnly BirthDate { get; set; }

    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    /// <summary>Optional 4-digit PIN stored as a hash. Soft profile-picker guard, NOT auth.</summary>
    public string? PinHash { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ChildCategoryLevel> CategoryLevels { get; set; } = new List<ChildCategoryLevel>();
    public ICollection<Progress> Progress { get; set; } = new List<Progress>();
}

/// <summary>
/// A child's current level for one specific category (e.g. Animals = A1).
/// Independent per category, so a child can be advanced in Letters
/// but a beginner in Animals.
/// </summary>
public sealed class ChildCategoryLevel
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;

    public Category Category { get; set; }
    public Level Level { get; set; }
}

public sealed class Lesson
{
    public int Id { get; set; }
    public Category Category { get; set; }
    public Level Level { get; set; }
    public required string TitleEn { get; set; }
    public required string TitleHe { get; set; }
    public int OrderInLevel { get; set; }

    public ICollection<LessonItem> Items { get; set; } = new List<LessonItem>();
}

public sealed class LessonItem
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public LessonItemKind Kind { get; set; }
    public int OrderInLesson { get; set; }

    public required string PromptEn { get; set; }
    public string? PromptHe { get; set; }

    /// <summary>
    /// Kind-specific payload as JSON. Schema validated client-side per kind.
    /// See ADR-0009 for examples.
    /// </summary>
    public required string PayloadJson { get; set; }
}

public sealed class Progress
{
    public int Id { get; set; }

    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;

    public int LessonItemId { get; set; }
    public LessonItem LessonItem { get; set; } = null!;

    /// <summary>0–100, last attempt's score.</summary>
    public int Score { get; set; }
    public int AttemptCount { get; set; }
    public DateTime FirstAttemptedAtUtc { get; set; }
    public DateTime LastAttemptedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
