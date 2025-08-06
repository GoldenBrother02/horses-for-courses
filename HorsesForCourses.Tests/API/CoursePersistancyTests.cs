using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.WebApi;
using Microsoft.Data.Sqlite;

namespace HorsesForCourses.Tests.Data;

public class CoursePersistancyTests
{
    [Fact]
    public async Task ShouldPersistData()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = new Coach(1, "naam", "em@il");
            coach.AddCompetence("dev");
            context.Coaches.Add(coach);

            var course = new Course(1, "cursus", new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1));
            course.AddRequirement("dev");
            course.AddCourseMoment(
                TimeSlot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)));
            course.ConfirmCourse();
            course.AddCoach(coach);
            context.Courses.Add(course);

            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches
                .Include(c => c.CourseList)
                .FirstOrDefaultAsync(c => c.Id == 1);

            Assert.NotNull(coach);
            Assert.Equal("naam", coach!.Name);
            Assert.NotNull(coach.CourseList);
            Assert.Single(coach.CourseList);

            var course = await context.Courses.FindAsync(1);
            Assert.NotNull(course);
            Assert.Equal("cursus", course!.CourseName);
            Assert.NotNull(course.coach);
        }
    }
}