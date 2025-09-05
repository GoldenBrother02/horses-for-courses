using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Moq;

namespace HorsesForCourses.Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockRepo;
    private readonly Mock<ICoachRepository> _mockCoachRepo;
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _mockRepo = new Mock<ICourseRepository>();
        _mockCoachRepo = new Mock<ICoachRepository>();
        _service = new CourseService(_mockRepo.Object, _mockCoachRepo.Object);
    }

    [Fact]
    public async Task GetCourseById_ReturnsCourse_WhenExists()
    {
        var course = new Course("Math", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync(course);

        var result = await _service.GetCourseById(1);

        Assert.Equal(course, result);
    }

    [Fact]
    public async Task CreateCourse_CallsRepoAndReturnsCourse()
    {
        var course = new Course("Math", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        _mockRepo.Setup(r => r.CreateCourse(course)).ReturnsAsync(course);

        var result = await _service.CreateCourse(course);

        Assert.Equal(course, result);
        _mockRepo.Verify(r => r.CreateCourse(course), Times.Once);
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task OverwriteRequirements_ReturnsFalse_WhenCourseDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync((Course?)null);

        var result = await _service.OverwriteRequirements(1, new List<string> { "C#" });

        Assert.False(result);
    }

    [Fact]
    public async Task OverwriteRequirements_ReturnsTrue_WhenCourseExists()
    {
        var course = new Course("Math", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync(course);

        var result = await _service.OverwriteRequirements(1, new List<string> { "C#" });

        Assert.True(result);
        Assert.Contains("C#", course.RequiredCompetencies);
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task AddCoach_ReturnsFalse_WhenCourseOrCoachDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync((Course?)null);

        var result = await _service.AddCoach(1, 1);
        Assert.False(result);

        var course = new Course("Math", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        _mockRepo.Setup(r => r.GetCourseById(2)).ReturnsAsync(course);
        _mockCoachRepo.Setup(r => r.GetCoachById(1)).ReturnsAsync((Coach?)null);

        var result2 = await _service.AddCoach(2, 1);
        Assert.False(result2);
    }

    [Fact]
    public async Task AddCoach_ReturnsTrueAndAddsBookingWithCorrectSlots()
    {
        var courseStart = DateOnly.FromDateTime(DateTime.Today);
        var courseEnd = courseStart.AddDays(24);

        var course = new Course("Math", courseStart, courseEnd);

        var slot1 = TimeSlot.From(DayOfWeek.Monday, TimeOnly.FromTimeSpan(new TimeSpan(9, 0, 0)), TimeOnly.FromTimeSpan(new TimeSpan(12, 0, 0)));
        var slot2 = TimeSlot.From(DayOfWeek.Wednesday, TimeOnly.FromTimeSpan(new TimeSpan(14, 0, 0)), TimeOnly.FromTimeSpan(new TimeSpan(16, 0, 0)));

        course.AddCourseMoment(slot1);
        course.AddCourseMoment(slot2);

        course.ConfirmCourse();
        course.AddRequirement("C#");

        var coach = new Coach("John", "john@example.com");
        coach.AddCompetence("C#");

        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync(course);
        _mockCoachRepo.Setup(r => r.GetCoachById(1)).ReturnsAsync(coach);

        var result = await _service.AddCoach(1, 1);

        Assert.True(result);
        Assert.Equal(States.FINALISED, course.Status);

        var booking = Assert.Single(coach.bookings);

        Assert.Contains(slot1, booking.Planning);
        Assert.Contains(slot2, booking.Planning);

        _mockRepo.Verify(r => r.Save(), Times.Once);
    }


    [Fact]
    public async Task ConfirmCourse_ReturnsFalse_WhenCourseDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync((Course?)null);

        var result = await _service.ConfirmCourse(1);

        Assert.False(result);
    }

    [Fact]
    public async Task ConfirmCourse_ReturnsTrue_WhenCourseExists()
    {
        var course = new Course("Math", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        course.AddCourseMoment(TimeSlot.From(DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("10:00")));

        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync(course);

        var result = await _service.ConfirmCourse(1);

        Assert.True(result);
        Assert.Equal(States.CONFIRMED, course.Status);
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAllCourses_ReturnsPagedResult()
    {
        var paged = new PagedResult<CourseDTO>(new List<CourseDTO>(), 0, 1, 10);
        _mockRepo.Setup(r => r.GetAllCourses(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await _service.GetAllCourses(1, 10);

        Assert.Equal(paged, result);
    }

    [Fact]
    public async Task OverwriteCourseMoments_ReturnsFalse_WhenCourseDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync((Course?)null);

        var result = await _service.OverwriteCourseMoments(1, new List<TimeSlotDTO>());

        Assert.False(result);
    }

    [Fact]
    public async Task OverwriteCourseMoments_ReturnsTrueAndUpdatesMoments_WhenCourseExists()
    {
        var course = new Course("Math", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(10)));
        _mockRepo.Setup(r => r.GetCourseById(1)).ReturnsAsync(course);

        var newMoments = new List<TimeSlotDTO>
        {
            new TimeSlotDTO(DayOfWeek.Monday, new TimeOnly(9,0), new TimeOnly(10,0)),
            new TimeSlotDTO(DayOfWeek.Wednesday, new TimeOnly(13,0), new TimeOnly(14,0))
        };

        var result = await _service.OverwriteCourseMoments(1, newMoments);

        Assert.True(result);
        Assert.Equal(2, course.Planning.Count);
        Assert.Contains(course.Planning, ts => ts.Day == DayOfWeek.Monday && ts.Start == new TimeOnly(9, 0));
        Assert.Contains(course.Planning, ts => ts.Day == DayOfWeek.Wednesday && ts.Start == new TimeOnly(13, 0));
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }
}
