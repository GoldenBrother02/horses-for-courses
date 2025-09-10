using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.MVC;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HorsesForCourses.Tests
{
    public class CoachesControllerTests
    {
        private readonly Mock<ICoachService> _mockService;
        private readonly CoachesController _controller;

        public CoachesControllerTests()
        {
            _mockService = new Mock<ICoachService>();
            _controller = new CoachesController(_mockService.Object);
        }

        private void AuthenticateController()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, "testuser@example.com") },
                "mockAuthType"
            ));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Fact]
        public async Task GetById_ReturnsView_WhenCoachExists()
        {
            var coach = new Coach("John Doe", "john@example.com");
            _mockService.Setup(s => s.GetCoachById(1)).ReturnsAsync(coach);

            var result = await _controller.GetById(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(coach, viewResult.Model);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _mockService.Setup(s => s.GetCoachById(1)).ReturnsAsync((Coach)null!);

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
        public async Task CreateCoach_InvalidModel_ReturnsCreateMenuView()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var post = new PostCoach { Name = "", Email = "test@example.com" };

            var result = await _controller.CreateCoach(post);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CreateMenu", viewResult.ViewName);
            Assert.Equal(post, viewResult.Model);
        }

        [Fact]
        public async Task CreateCoach_ValidModel_RedirectsToGetById()
        {
            var post = new PostCoach { Name = "John Doe", Email = "john@example.com" };

            var createdCoach = new Coach(post.Name, post.Email);
            createdCoach.GetType().GetProperty("Id")!.SetValue(createdCoach, 42);

            _mockService.Setup(s => s.CreateCoach(It.IsAny<Coach>()))
                        .ReturnsAsync(createdCoach);

            var result = await _controller.CreateCoach(post);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoachesController.GetById), redirect.ActionName);
            Assert.Equal(42, redirect.RouteValues!["id"]);
        }


        [Fact]
        public async Task EditMenu_ReturnsView_WhenCoachExists()
        {
            AuthenticateController();

            var coach = new Coach("Jane", "jane@example.com");
            coach.AddCompetence("Running");

            _mockService.Setup(s => s.GetCoachById(1)).ReturnsAsync(coach);

            var result = await _controller.EditMenu(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditCoachSkills>(viewResult.Model);
            Assert.Equal(coach.Id, model.CoachId);
            Assert.Contains("Running", model.Skills);
        }

        [Fact]
        public async Task EditMenu_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _mockService.Setup(s => s.GetCoachById(1)).ReturnsAsync((Coach)null!);

            var result = await _controller.EditMenu(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditSkills_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("Skills", "Required");
            var model = new EditCoachSkills { CoachId = 1, Skills = new List<string>() };

            var result = await _controller.EditSkills(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task EditSkills_Success_RedirectsToGetById()
        {
            AuthenticateController();

            var model = new EditCoachSkills { CoachId = 1, Skills = new List<string> { "Running" } };
            _mockService.Setup(s => s.OverwriteCoachSkillset(1, model.Skills)).ReturnsAsync(true);

            var result = await _controller.EditSkills(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoachesController.GetById), redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task EditSkills_Failure_ReturnsNotFound()
        {
            AuthenticateController();

            var model = new EditCoachSkills { CoachId = 1, Skills = new List<string> { "Swimming" } };
            _mockService.Setup(s => s.OverwriteCoachSkillset(1, model.Skills)).ReturnsAsync(false);

            var result = await _controller.EditSkills(model);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCoaches()
        {
            var coachDtos = new List<CoachDTO>{
                new CoachDTO(
                    1,
                    "John",
                    "john@example.com",
                    new List<string> { "Running" },
                    new List<IdNameCourse>()
                    ),

                new CoachDTO(
                    2,
                    "Jane",
                    "jane@example.com",
                    new List<string> { "Swimming" },
                    new List<IdNameCourse>()
                    )
            };

            var pagedResult = new PagedResult<CoachDTO>(
                Items: coachDtos,
                TotalCount: coachDtos.Count,
                PageNumber: 1,
                PageSize: 5
            );

            _mockService.Setup(s => s.GetAllCoaches(1, 5, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(pagedResult);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PagedResult<CoachDTO>>(viewResult.Model);
            Assert.Equal(pagedResult, model);
        }
    }
}
