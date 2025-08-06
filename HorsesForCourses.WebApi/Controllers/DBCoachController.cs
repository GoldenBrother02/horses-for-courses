using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("api/Coaches")]
public class DBCoachController : ControllerBase
{
    private readonly CoachMapper _coachMap;
    private readonly CourseMapper _courseMap;
    private readonly TimeSlotMapper _timeSlotDTOMap;
    private readonly AppDbContext _context;

    public DBCoachController(CoachMapper coachMap, CourseMapper coursemap, TimeSlotMapper timeslotDTOMap, AppDbContext context)
    {
        _coachMap = coachMap;
        _courseMap = coursemap;
        _timeSlotDTOMap = timeslotDTOMap;
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Coach>> GetCoachById(int id)
    {
        var coach = await _context.Coaches.Include(c => c.CourseList).FirstOrDefaultAsync(e => e.Id == id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost]
    public async Task<ActionResult<Coach>> CreateCoach([FromBody] PostCoach post)
    {
        var result = _coachMap.Map(post, _coachMap.GetNextId(_context) + 1);
        _context.Coaches.Add(result);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCoachById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteCoachSkillset(int id, [FromBody] List<string> NewSkills)
    {
        var coach = await _context.Coaches.FirstOrDefaultAsync(e => e.Id == id);
        if (coach is null) { return NotFound(); }

        coach.OverwriteCompetenties(NewSkills);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Coach>>> GetAllCoaches()
    {
        var list = await _context.Coaches.ToListAsync();
        return Ok(list);
    }
}