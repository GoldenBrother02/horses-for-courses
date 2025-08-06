using System;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("api/Courses")]
public class DBCourseController : ControllerBase
{
    private readonly CoachMapper _coachMap;
    private readonly CourseMapper _courseMap;
    private readonly TimeSlotMapper _timeSlotDTOMap;
    private readonly AppDbContext _context;
    public DBCourseController(CoachMapper coachMap, CourseMapper coursemap, TimeSlotMapper timeslotDTOMap, AppDbContext context)
    {
        _coachMap = coachMap;
        _courseMap = coursemap;
        _timeSlotDTOMap = timeslotDTOMap;
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourseById(int id)
    {
        var course = await _context.Courses.Include(c => c.Planning).FirstOrDefaultAsync(e => e.Id == id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<Coach>> CreateCourse([FromBody] PostCourse post)
    {
        var result = _courseMap.PostToCourse(post, _courseMap.GetNextId(_context) + 1);
        _context.Courses.Add(result);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteRequirements(int id, [FromBody] List<string> NewSkills)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(e => e.Id == id);
        if (course is null) { return NotFound(); }

        course.OverwriteRequirements(NewSkills);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
    {
        var list = await _context.Courses.ToListAsync();
        return Ok(list);
    }

    [HttpPost("{id}/timeslots")]
    public async Task<ActionResult> OverwriteCourseMoments(int id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(e => e.Id == id);
        if (course is null) { return NotFound(); }

        var list = _timeSlotDTOMap.Map(NewMoments);

        course.OverwriteMoments(list);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(e => e.Id == id);
        if (course is null) { return NotFound(); }

        course.ConfirmCourse();
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{CourseId}/assign-coach")]
    public async Task<ActionResult> AddCoach(int CourseId, int CoachId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(e => e.Id == CourseId);
        if (course is null) { return NotFound(); }

        var coach = await _context.Coaches.FirstOrDefaultAsync(e => e.Id == CoachId);
        if (coach is null) { return NotFound(); }

        course.AddCoach(coach);
        await _context.SaveChangesAsync();

        return Ok();
    }
}