namespace EzEnglish.Api.Domain.Enums;

/// <summary>
/// Type of interaction a <see cref="Entities.LessonItem"/> represents.
/// The frontend has one React component per kind that knows how to render
/// the item's <c>PayloadJson</c>. Adding a new kind requires a new enum
/// value and a new React component — no schema migration needed.
/// </summary>
public enum LessonItemKind
{
    DragMatch   = 1,
    TapPick     = 2,
    LetterTrace = 3,
    Memory      = 4,
    Sort        = 5,
}
