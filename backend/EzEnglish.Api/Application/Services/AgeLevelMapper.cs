using EzEnglish.Api.Domain.Enums;

namespace EzEnglish.Api.Application.Services;

/// <summary>
/// Maps a child's age (in years, computed from BirthDate) to an initial level
/// per category, per ADR-0008.
/// </summary>
public static class AgeLevelMapper
{
    public static Level StartingLevelForAge(int ageYears) => ageYears switch
    {
        <= 4  => Level.PreA1,
        5 or 6 => Level.A0,
        7 or 8 => Level.A1,
        9 or 10 => Level.A2,
        _ => Level.B1,
    };

    public static int AgeYears(DateOnly birthDate, DateOnly today)
    {
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        return Math.Max(0, age);
    }

    public static IEnumerable<Category> AllCategories =>
        Enum.GetValues<Category>();
}
