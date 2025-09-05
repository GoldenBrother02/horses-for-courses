using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.MVC;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HorsesForCourses.Tests
{
    public class CoursesControllerTests
    {
        private readonly Mock<ICourseService> _mockService;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mockService = new Mock<ICourseService>();
            _controller = new CoursesController(_mockService.Object);
        }

        [Fact]
        public async Task GetById_ReturnsView_WhenCourseExists()
        {
            var course = new Course("Math 101", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
            _mockService.Setup(s => s.GetCourseById(1)).ReturnsAsync(course);

            var result = await _controller.GetById(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(course, viewResult.Model);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _mockService.Setup(s => s.GetCourseById(1)).ReturnsAsync((Course)null!);

            var result = await _controller.GetById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateMenu_ReturnsView()
        {
            var result = _controller.CreateMenu();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreateCourse_InvalidModel_ReturnsCreateMenuView()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var post = new PostCourse { Name = "", Start = DateOnly.FromDateTime(DateTime.Today), End = DateOnly.FromDateTime(DateTime.Today) };

            var result = await _controller.CreateCourse(post);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateMenu", viewResult.ViewName);
            Assert.Equal(post, viewResult.Model);
        }

        [Fact]
        public async Task CreateCourse_ValidModel_RedirectsToGetById()
        {
            var post = new PostCourse
            {
                Name = "Math 101",
                Start = DateOnly.FromDateTime(DateTime.Today),
                End = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            var createdCourse = new Course(post.Name, post.Start, post.End);
            createdCourse.GetType().GetProperty("Id")!.SetValue(createdCourse, 42);

            _mockService.Setup(s => s.CreateCourse(It.IsAny<Course>()))
                        .ReturnsAsync(createdCourse);

            var result = await _controller.CreateCourse(post);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoursesController.GetById), redirect.ActionName);
            Assert.Equal(42, redirect.RouteValues!["id"]);
        }


        [Fact]
        public async Task Index_ReturnsViewWithPagedCourses()
        {
            var courses = new List<CourseDTO>
    {
        new CourseDTO(
            name: "Math 101",
            start: DateOnly.FromDateTime(DateTime.Today),
            end: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            id: 1,
            status: States.PENDING,
            competenties: new List<string>(),
            timeSlots: new List<TimeSlotDTO>(),
            coach: null
        ),
        new CourseDTO(
            name: "Physics",
            start: DateOnly.FromDateTime(DateTime.Today),
            end: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            id: 2,
            status: States.PENDING,
            competenties: new List<string>(),
            timeSlots: new List<TimeSlotDTO>(),
            coach: null
        )
    };

            var pagedResult = new PagedResult<CourseDTO>(courses, courses.Count, 1, 10);

            _mockService.Setup(s => s.GetAllCourses(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(pagedResult);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PagedResult<CourseDTO>>(viewResult.Model);
            Assert.Equal(pagedResult, model);
        }


        [Fact]
        public async Task EditMenu_ReturnsView_WhenCourseExists()
        {
            var course = new Course("Math 101", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
            course.AddRequirement("Algebra");

            _mockService.Setup(s => s.GetCourseById(1)).ReturnsAsync(course);
            _mockService.Setup(s => s.TimeSlotListToTimeSlotDTOList(It.IsAny<List<TimeSlot>>())).Returns(new List<TimeSlotDTO>());

            var result = await _controller.EditMenu(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditCourse>(viewResult.Model);
            Assert.Equal(course.Id, model.CourseId);
            Assert.Contains("Algebra", model.Skills);
        }

        [Fact]
        public async Task EditMenu_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _mockService.Setup(s => s.GetCourseById(1)).ReturnsAsync((Course)null!);

            var result = await _controller.EditMenu(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditSkills_Success_RedirectsToGetById()
        {
            var model = new EditCourse { CourseId = 1, Skills = new List<string> { "Algebra" } };
            _mockService.Setup(s => s.OverwriteRequirements(1, model.Skills)).ReturnsAsync(true);

            var result = await _controller.EditSkills(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoursesController.GetById), redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task EditSkills_Failure_ReturnsNotFound()
        {
            var model = new EditCourse { CourseId = 1, Skills = new List<string> { "Algebra" } };
            _mockService.Setup(s => s.OverwriteRequirements(1, model.Skills)).ReturnsAsync(false);

            var result = await _controller.EditSkills(model);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditMoments_Success_RedirectsToGetById()
        {
            var model = new EditCourse { CourseId = 1, Moments = new List<TimeSlotDTO>() };
            _mockService.Setup(s => s.OverwriteCourseMoments(1, model.Moments)).ReturnsAsync(true);

            var result = await _controller.EditMoments(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoursesController.GetById), redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task EditMoments_Failure_ReturnsNotFound()
        {
            var model = new EditCourse { CourseId = 1, Moments = new List<TimeSlotDTO>() };
            _mockService.Setup(s => s.OverwriteCourseMoments(1, model.Moments)).ReturnsAsync(false);

            var result = await _controller.EditMoments(model);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Confirm_Success_RedirectsToIndex()
        {
            _mockService.Setup(s => s.ConfirmCourse(1)).ReturnsAsync(true);

            var result = await _controller.Confirm(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoursesController.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task Confirm_Failure_ReturnsNotFound()
        {
            _mockService.Setup(s => s.ConfirmCourse(1)).ReturnsAsync(false);

            var result = await _controller.Confirm(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddCoachMenu_ReturnsView_WhenCourseExists()
        {
            var course = new Course("Math 101", DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
            _mockService.Setup(s => s.GetCourseById(1)).ReturnsAsync(course);

            var result = await _controller.AddCoachMenu(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddCoachToCourse>(viewResult.Model);
            Assert.Equal(course.Id, model.CourseId);
            Assert.Equal(-1, model.CoachId);
        }

        [Fact]
        public async Task AddCoach_Success_RedirectsToIndex()
        {
            var model = new AddCoachToCourse { CourseId = 1, CoachId = 2 };
            _mockService.Setup(s => s.AddCoach(model.CourseId, model.CoachId)).ReturnsAsync(true);

            var result = await _controller.AddCoach(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoursesController.Index), redirectResult.ActionName);
        }

        [Fact]
        public async Task AddCoach_Failure_ReturnsNotFound()
        {
            var model = new AddCoachToCourse { CourseId = 1, CoachId = 2 };
            _mockService.Setup(s => s.AddCoach(model.CourseId, model.CoachId)).ReturnsAsync(false);

            var result = await _controller.AddCoach(model);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
