using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class AppUserCrudTests : CrudTestBase<AppDbContext, AppUser>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options) => new AppDbContext(options);

    protected override AppUser CreateEntity()
        => AppUser.From("test", "test@mail.com", "1234", "1234", "role");

    protected override DbSet<AppUser> GetDbSet(AppDbContext context)
        => context.Users;

    protected override object[] GetPrimaryKey(AppUser entity)
        => new object[] { entity.Id };

    protected override async Task ModifyEntityAsync(AppUser entity)
    {
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(AppUser entity)
    {
        await Task.CompletedTask;
    }
}