using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Moq;
using Xunit;

namespace HorsesForCourses.Tests;

public class CoachServiceTests
{
    private readonly Mock<ICoachRepository> _mockRepo;
    private readonly CoachService _service;

    public CoachServiceTests()
    {
        _mockRepo = new Mock<ICoachRepository>();
        _service = new CoachService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetCoachById_ReturnsCoach_WhenExists()
    {
        var coach = new Coach("John", "john@example.com");
        _mockRepo.Setup(r => r.GetCoachById(1)).ReturnsAsync(coach);

        var result = await _service.GetCoachById(1);

        Assert.Equal(coach, result);
    }

    [Fact]
    public async Task CreateCoach_CallsRepoAndReturnsCoach()
    {
        var coach = new Coach("John", "john@example.com");
        _mockRepo.Setup(r => r.CreateCoach(coach)).ReturnsAsync(coach);

        var result = await _service.CreateCoach(coach);

        Assert.Equal(coach, result);
        _mockRepo.Verify(r => r.CreateCoach(coach), Times.Once);
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task OverwriteCoachSkillset_ReturnsFalse_WhenCoachDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetCoachById(1)).ReturnsAsync((Coach?)null);

        var result = await _service.OverwriteCoachSkillset(1, new List<string> { "C#" });

        Assert.False(result);
    }

    [Fact]
    public async Task OverwriteCoachSkillset_ReturnsTrue_WhenCoachExists()
    {
        var coach = new Coach("John", "john@example.com");
        _mockRepo.Setup(r => r.GetCoachById(1)).ReturnsAsync(coach);

        var result = await _service.OverwriteCoachSkillset(1, new List<string> { "C#" });

        Assert.True(result);
        Assert.Contains("C#", coach.competencies);
        _mockRepo.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAllCoaches_ReturnsPagedResult()
    {
        var paged = new PagedResult<CoachDTO>(new List<CoachDTO>(), 0, 1, 10);
        _mockRepo.Setup(r => r.GetAllCoaches(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        var result = await _service.GetAllCoaches(1, 10);

        Assert.Equal(paged, result);
    }
}
