using EzEnglish.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EzEnglish.Api.Infrastructure.Persistence.Configurations;

internal sealed class ParentConfig : IEntityTypeConfiguration<Parent>
{
    public void Configure(EntityTypeBuilder<Parent> b)
    {
        b.ToTable("Parents");
        b.HasKey(p => p.Id);
        b.Property(p => p.FirebaseUid).IsRequired().HasMaxLength(128);
        b.Property(p => p.Email).IsRequired().HasMaxLength(320);
        b.Property(p => p.DisplayName).HasMaxLength(128);
        b.HasIndex(p => p.FirebaseUid).IsUnique();
        b.HasIndex(p => p.Email);
    }
}

internal sealed class CharacterConfig : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> b)
    {
        b.ToTable("Characters");
        b.HasKey(c => c.Id);
        b.Property(c => c.Key).IsRequired().HasMaxLength(64);
        b.Property(c => c.DisplayNameEn).IsRequired().HasMaxLength(128);
        b.Property(c => c.DisplayNameHe).IsRequired().HasMaxLength(128);
        b.Property(c => c.AvatarUrl).IsRequired().HasMaxLength(512);
        b.HasIndex(c => c.Key).IsUnique();
    }
}

internal sealed class ChildConfig : IEntityTypeConfiguration<Child>
{
    public void Configure(EntityTypeBuilder<Child> b)
    {
        b.ToTable("Children");
        b.HasKey(c => c.Id);
        b.Property(c => c.DisplayName).IsRequired().HasMaxLength(64);
        b.Property(c => c.PinHash).HasMaxLength(256);
        b.HasIndex(c => c.ParentId);
        b.HasOne(c => c.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(c => c.Character)
            .WithMany()
            .HasForeignKey(c => c.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ChildCategoryLevelConfig : IEntityTypeConfiguration<ChildCategoryLevel>
{
    public void Configure(EntityTypeBuilder<ChildCategoryLevel> b)
    {
        b.ToTable("ChildCategoryLevels");
        b.HasKey(x => x.Id);
        b.HasOne(x => x.Child)
            .WithMany(c => c.CategoryLevels)
            .HasForeignKey(x => x.ChildId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.ChildId, x.Category }).IsUnique();
    }
}

internal sealed class LessonConfig : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> b)
    {
        b.ToTable("Lessons");
        b.HasKey(l => l.Id);
        b.Property(l => l.TitleEn).IsRequired().HasMaxLength(256);
        b.Property(l => l.TitleHe).IsRequired().HasMaxLength(256);
        b.HasIndex(l => new { l.Category, l.Level, l.OrderInLevel });
    }
}

internal sealed class LessonItemConfig : IEntityTypeConfiguration<LessonItem>
{
    public void Configure(EntityTypeBuilder<LessonItem> b)
    {
        b.ToTable("LessonItems");
        b.HasKey(i => i.Id);
        b.Property(i => i.PromptEn).IsRequired().HasMaxLength(1024);
        b.Property(i => i.PromptHe).HasMaxLength(1024);
        b.Property(i => i.PayloadJson).IsRequired();
        b.HasOne(i => i.Lesson)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(i => new { i.LessonId, i.OrderInLesson });
    }
}

internal sealed class ProgressConfig : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> b)
    {
        b.ToTable("Progress");
        b.HasKey(p => p.Id);
        b.HasOne(p => p.Child)
            .WithMany(c => c.Progress)
            .HasForeignKey(p => p.ChildId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(p => p.LessonItem)
            .WithMany()
            .HasForeignKey(p => p.LessonItemId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(p => new { p.ChildId, p.LessonItemId }).IsUnique();
    }
}
