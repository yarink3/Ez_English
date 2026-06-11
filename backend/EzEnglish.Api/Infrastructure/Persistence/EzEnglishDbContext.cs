using EzEnglish.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EzEnglish.Api.Infrastructure.Persistence;

public sealed class EzEnglishDbContext(DbContextOptions<EzEnglishDbContext> options) : DbContext(options)
{
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<ChildCategoryLevel> ChildCategoryLevels => Set<ChildCategoryLevel>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonItem> LessonItems => Set<LessonItem>();
    public DbSet<Progress> Progress => Set<Progress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EzEnglishDbContext).Assembly);
        SeedData.Seed(modelBuilder);
    }
}
