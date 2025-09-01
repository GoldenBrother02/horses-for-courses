using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class CourseCrudTests : CrudTestBase<AppDbContext, Course>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options) => new AppDbContext(options);

    protected override Course CreateEntity() => new Course("test", new DateOnly(2025, 6, 6), new DateOnly(2026, 6, 6));

    protected override DbSet<Course> GetDbSet(AppDbContext context) => context.Courses;

    protected override object[] GetPrimaryKey(Course entity) => new object[] { entity.Id };

    protected override async Task ModifyEntityAsync(Course entity)
    {
        entity.OverwriteRequirements(new List<string> { "unit", "test" });
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(Course entity)
    {
        Assert.Contains("unit", entity.RequiredCompetencies);
        Assert.Contains("test", entity.RequiredCompetencies);
        await Task.CompletedTask;
    }
}