using HorsesForCourses.Core;
using HorsesForCourses.WebApi;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class CoachCrudTests : CrudTestBase<AppDbContext, Coach>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options) => new AppDbContext(options);

    protected override Coach CreateEntity()
        => new Coach("Unit Test", "unit@test.com");

    protected override DbSet<Coach> GetDbSet(AppDbContext context)
        => context.Coaches;

    protected override object[] GetPrimaryKey(Coach entity)
        => new object[] { entity.Id };

    protected override async Task ModifyEntityAsync(Coach entity)
    {
        entity.OverwriteCompetenties(new List<string> { "unit", "test" });
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(Coach entity)
    {
        Assert.Contains("unit", entity.competencies);
        Assert.Contains("test", entity.competencies);
        await Task.CompletedTask;
    }
}