using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("api/Coaches")]
public class DBCoachController : ControllerBase
{
    private readonly ICoachService _service;

    public DBCoachController(ICoachService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Coach>> GetCoachById(int id)
    {
        var coach = await _service.GetCoachById(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost]
    public async Task<ActionResult<Coach>> CreateCoach([FromBody] PostCoach post)
    {
        var result = new Coach(post.Name, post.Email);

        await _service.CreateCoach(result);

        return CreatedAtAction(nameof(GetCoachById), new { id = result.Id }, result);

    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteCoachSkillset(int id, [FromBody] List<string> NewSkills)
    {
        var success = await _service.OverwriteCoachSkillset(id, NewSkills);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CoachDTO>>> GetAllCoaches(
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    CancellationToken ct = default)
    {
        var result = await _service.GetAllCoaches(page, size, ct);
        return Ok(result);
    }
}