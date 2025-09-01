using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi;

[ApiController]
[Route("api/Courses")]
public class DBCourseController : ControllerBase
{
    private readonly ICourseService _service;
    public DBCourseController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourseById(int id)
    {
        var course = await _service.GetCourseById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<Course>> CreateCourse([FromBody] PostCourse post)
    {
        var result = new Course(post.Name, post.Start, post.End);

        await _service.CreateCourse(result);

        return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/Competencies")]
    public async Task<ActionResult> OverwriteRequirements(int id, [FromBody] List<string> NewSkills)
    {
        var success = await _service.OverwriteRequirements(id, NewSkills);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseDTO>>> GetAllCourses(
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    CancellationToken ct = default)
    {
        var list = await _service.GetAllCourses(page, size, ct);
        return Ok(list);
    }

    [HttpPost("{id}/timeslots")]
    public async Task<ActionResult> OverwriteCourseMoments(int id, [FromBody] List<TimeSlotDTO> NewMoments)
    {
        var success = await _service.OverwriteCourseMoments(id, NewMoments);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var success = await _service.ConfirmCourse(id);

        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpPost("{CourseId}/assign-coach")]
    public async Task<ActionResult> AddCoach(int CourseId, int CoachId)
    {
        var success = await _service.AddCoach(CourseId, CoachId);

        if (success) { return Ok(); }
        return NotFound();
    }
}