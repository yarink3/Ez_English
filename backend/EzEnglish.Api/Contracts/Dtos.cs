using System.Text.Json;
using EzEnglish.Api.Domain.Enums;

namespace EzEnglish.Api.Contracts;

public sealed record ParentDto(int Id, string Email, string? DisplayName);

public sealed record CharacterDto(int Id, string Key, string DisplayNameEn, string DisplayNameHe, string AvatarUrl);

public sealed record ChildDto(
    int Id,
    string DisplayName,
    DateOnly BirthDate,
    int CharacterId,
    IReadOnlyList<ChildCategoryLevelDto> CategoryLevels);

public sealed record ChildCategoryLevelDto(Category Category, Level Level);

public sealed record CreateChildRequest(
    string DisplayName,
    DateOnly BirthDate,
    int CharacterId,
    string? Pin);

public sealed record MeResponse(ParentDto Parent, IReadOnlyList<ChildDto> Children);

public sealed record LessonSummaryDto(
    int Id,
    Category Category,
    Level Level,
    string TitleEn,
    string TitleHe,
    int OrderInLevel,
    int ItemCount);

public sealed record LessonItemDto(
    int Id,
    LessonItemKind Kind,
    int OrderInLesson,
    string PromptEn,
    string? PromptHe,
    JsonElement Payload);

public sealed record LessonDetailDto(
    int Id,
    Category Category,
    Level Level,
    string TitleEn,
    string TitleHe,
    IReadOnlyList<LessonItemDto> Items);

public sealed record SubmitProgressRequest(int LessonItemId, int Score);

public sealed record ProgressDto(
    int LessonItemId,
    int Score,
    int AttemptCount,
    DateTime FirstAttemptedAtUtc,
    DateTime LastAttemptedAtUtc,
    DateTime? CompletedAtUtc);

