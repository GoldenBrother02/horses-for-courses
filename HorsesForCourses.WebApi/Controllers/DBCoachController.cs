using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("api/Coaches")]
public class DBCoachController : ControllerBase
{
    private readonly CoachRepository _repo;

    public DBCoachController(CoachRepository repository)
    {
        _repo = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Coach>> GetCoachById(int id)
    {
        var coach = await _repo.GetCoachById(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost]
    public async Task<ActionResult<Coach>> CreateCoach([FromBody] PostCoach post)
    {
        var result = await _repo.CreateCoach(post);
        return CreatedAtAction(nameof(GetCoachById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteCoachSkillset(int id, [FromBody] List<string> NewSkills)
    {
        var coach = await _repo.GetCoachById(id);
        if (coach is null) { return NotFound(); }

        await _repo.OverwriteCoachSkillset(coach, NewSkills);

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CoachDTO>>> GetAllCoaches(
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    CancellationToken ct = default)
    {
        var result = await _repo.GetAllCoaches(page, size, ct);
        return Ok(result);
    }
}